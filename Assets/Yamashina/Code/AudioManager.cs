using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// ゲーム全体の音声再生を管理するシングルトンコンポーネント。
/// BGM と SE（効果音）の再生、停止、ボリューム調整などを行う。
/// </summary>
public class AudioManager : SingletonMonoBehaviour<AudioManager>
{
    #region オーディオソース (コード管理／Inspector非表示)内部管理用変数

    /// <summary>
    /// BGM 再生用 AudioSource（コードで生成）
    /// </summary>
    private AudioSource bgmSource;

    /// <summary>
    /// 同時再生用 SE AudioSource 配列（コードで生成）
    /// </summary>
    private AudioSource[] seSources;

    #endregion


    #region BGMの内部管理用変数

    /// <summary>
    ///  現在の BGM 音量 (0.0 - 1.0)
    /// </summary>
    private float bgmVolume;

    /// <summary>
    /// 現在の SE 音量 (0.0 - 1.0)
    /// </summary>
    private float seVolume;

    #endregion


    #region 各音源テーブルの内部管理用変数

    // <summary>
    //登録されている BGM 設定テーブル
    //ScriptableObjectとして保持（元データ）
    // </summary>
    private BGMConfigTable bgmConfigTable;

    /// <summary>
    /// 登録されている SE 設定テーブル
    /// ScriptableObjectとして保持（元データ）
    /// </summary>
    private SEConfigTable seConfigTable;

    #endregion


    #region その他の内部管理用変数    

    /// 現在流れているBGMのID
    /// </summary>
    private BGMName currentBgmId;

    /// <summary>
    /// スタート時のオーディオソースの現在時刻（DspTime)
    /// </summary>
    private double bgmStartDspTime;

    /// <summary>
    /// ゲームの初期設定
    /// </summary>
    private GameSettings gameSettings;

    #endregion


    #region ゲッターメソッド

    /// <summary>
    ///  現在の BGM 再生位置（秒）を返す。
    /// </summary>
    /// <returns>float</returns>
    internal float GetCurrentBGMTime()
    {
        //再生中ではない
        if (bgmSource == null || bgmSource.clip == null || !bgmSource.isPlaying)
            return 0f;

        //オーディオシステムの現在時刻-スタート時のオーディオソースのDspTimeを引いて経過時間を計算して返す
        return (float)(AudioSettings.dspTime - bgmStartDspTime);
    }

    /// <summary>
    /// 現在設定されている BGM 音量を返す。
    /// </summary>
    /// <returns>float</returns>
    internal float GetBGMVolume() => bgmVolume;

    /// <summary>
    /// 現在設定されている SE 音量を返す。
    /// </summary>
    /// <returns>float</returns>
    internal float GetSEVolume() => seVolume;

    /// <summary>
    /// BGMが鳴り終わったかどうかを返す
    /// </summary>
    /// <returns>bool</returns>
    internal bool IsBGMFinished()
    {
        //BGMが鳴り終わったかを判定
        return bgmSource != null && !bgmSource.isPlaying && bgmSource.time > 0;
    }

    #endregion


    #region セッターメソッド

    /// <summary>
    /// BGM 音量を設定する (0.0 - 1.0)。
    /// </summary>
    /// <param name="volume">BGM音量</param>
    internal void SetBGMVolume(float volume)
    {
        // 引数で渡された音量値を0未満や1を超える値にならないように制限し、
        // その制限された値を bgmVolumeに代入してBGMの音量を設定する
        bgmVolume = Mathf.Clamp01(volume);

        //音量をBGMのオーディオソースに適応
        ApplyVolumes();
    }

    /// <summary>
    /// SE 音量を設定する (0.0 - 1.0)。
    /// </summary>
    /// <param name="volume">SE音量</param>
    internal void SetSFXVolume(float volume)
    {
        // 引数で渡された音量値を0未満や1を超える値にならないように制限し、
        // その制限された値をseVolumeに代入してSEの音量を設定する
        seVolume = Mathf.Clamp01(volume);

        //音量をSEのオーディオソースに適応
        ApplyVolumes();
    }

    // <summary>
    //BGM 設定テーブルを登録する。
    //</summary>
    /// <param name="bgmTable">ScriptableObject で用意した BGM 設定テーブル</param>
    internal void SetupBGMConfigTable(BGMConfigTable bgmTable)
    {
        bgmConfigTable = bgmTable;
    }

    /// <summary>
    /// SE 設定テーブルを登録する。
    /// </summary>
    /// <param name="seTable">ScriptableObject で用意した SE 設定テーブル</param>
    internal void SetupSEConfigTable(SEConfigTable seTable)
    {
        seConfigTable = seTable;
    }

    #endregion



    /// <summary>
    /// 初期化処理：AudioSource のセットアップと初期音量を設定する。
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        Debug.Log("AudioManager Awake");
        gameSettings = GameInitializer.Instance.GetGameSettings();
        InitializeAudioSources();
        InitializeAudioVolumes();
    }


    #region ゲッター、セッター以外の外部で呼び出し可能な関数（オーディオ関連)

    /// <summary>
    /// 指定されたBGMが未再生または異なる場合に再生を開始する
    /// </summary>
    /// <param name = "bgmId" > BGMConfigTable に登録された識別子</param>
    internal void PlayBGMIfNotPlaying(BGMName bgmId)
    {
        if (string.IsNullOrEmpty(bgmId.ToString())) return;

        // 同じ曲が流れていれば何もしない
        if (currentBgmId == bgmId && bgmSource.isPlaying)
        {
            return;
        }
        //ループ対応ありでBGMIDの楽曲を流す
        PlayBGMById(bgmId, islooped: true, forceReplay: false);
    }

    ///// <summary>
    ///// 指定された BGM を強制的に初めから再生し、ループしない設定にする。
    ///// </summary>
    ///// <param name="bgmId">BGMConfigTable に登録された識別子</param>
    //internal void ForcePlayBGM(BGMName bgmId)
    //{
    //    //BGMIDに何も入ってないなら処理しない
    //    if (string.IsNullOrEmpty(bgmId.ToString())) return;

    //    //ループ対応なしでBGMIDの楽曲をはじめから流す
    //    PlayBGMById(bgmId,islooped :false,forceReplay: true);
    //}

    /// <summary>
    /// 現在再生中の BGM を停止する。
    /// </summary>
    internal void StopBGM()
    {
        //BGMが再生中なら
        if (bgmSource.isPlaying)
        {
            //再生停止
            bgmSource.Stop();
        }
    }

    /// <summary>
    /// 指定した SE ID の効果音を再生する。
    /// </summary>
    /// <param name="seId">SEConfigTable に登録された識別子</param>
    internal void PlaySEById(SEName seId)
    {
        //SEテーブルに何も入ってないなら
        if (seConfigTable == null)
        {

            return;
        }
        //SEIDのデータをテーブルから取得する
        var seConfig = seConfigTable.GetSeConfig(seId);

        ///SEIDのデータがないなら
        if (seConfig == null)
        {
            return;
        }

        //SEを流す
        PlayClipsMultiAudioSources(seSources, seConfig.SeAudioClip);
    }

    #endregion


    #region プライベートメソッド

    /// <summary>
    /// AudioSource を初期化し、BGM と SE 用を生成する。
    /// </summary>
    private void InitializeAudioSources()
    {
        //  BGMとSEのオーディオソースを必要な数分新規生成
        bgmSource = gameObject.AddComponent<AudioSource>();
        seSources = new AudioSource[gameSettings.MaxSeCount];
        for (int i = 0; i < gameSettings.MaxSeCount; i++)
            seSources[i] = gameObject.AddComponent<AudioSource>();
    }

    /// <summary>
    /// BGM、SEの音量を初期化
    /// </summary>
    private void InitializeAudioVolumes()
    {
        bgmVolume = Mathf.Clamp01(gameSettings.InitialBgmVolume);
        seVolume = Mathf.Clamp01(gameSettings.InitialSeVolume);
        ApplyVolumes();
    }

    /// <summary>
    /// 単一の AudioSource でクリップを再生する処理。
    /// </summary>
    private void PlayClips(AudioSource source, AudioClip clip, bool loop = false, bool forceReplay = false)
    {
        //クリップに何も入ってこないなら
        if (clip == null)
        {
            return;
        }

        //プレイ中、強制再再生なしなら処理しない
        if (!forceReplay && source.clip == clip && source.isPlaying) return;

        source.clip = clip;
        source.loop = loop;

        bgmStartDspTime = AudioSettings.dspTime;

        source.Play();
    }

    /// <summary>
    /// 複数の AudioSource のいずれかで効果音を再生する共通処理。
    /// </summary>
    private void PlayClipsMultiAudioSources(AudioSource[] sources, AudioClip clip)
    {
        //クリップに何も入ってこないなら

        if (clip == null)
        {
            return;
        }

        //オーディオソースの中から一個ずつ取り出して
        foreach (var src in sources)
        {
            //そのオーディオソースが再生中ではないなら
            if (!src.isPlaying)
            {
                //SEを一度流して処理しない
                src.PlayOneShot(clip);
                return;
            }
        }

        // 全て使用中なら先頭で再生
        sources[0].PlayOneShot(clip);
    }

    /// <summary>
    /// 指定した BGM ID の曲をループ再生する。
    /// </summary>
   // / <param name = "bgmId" > BGMConfigTable に登録された識別子</param>
    /// <param name = "forceReplay" > 最初からBGMを流しなおすかどうか </ param >
    /// < param name="islooped">ループ対応させるかどうか</param>
    private void PlayBGMById(BGMName bgmId, bool islooped, bool forceReplay = false)
    {
        //BGMIDのデータをテーブルから取得する
        var bgmConfig = bgmConfigTable.GetBgmConfig(bgmId);

        //BGMIDのデータが見つからないなら
        if (bgmConfig == null)
        {
            return;
        }

        //BGMソースなどのデータを渡して終了
        PlayClips(bgmSource, bgmConfig.BgmAudioClip, loop: islooped, forceReplay: forceReplay);

        //現在のBGMIDを登録
        currentBgmId = bgmId;
        Debug.Log(currentBgmId);
    }

    /// <summary>
    /// 設定した BGM と SE の音量を適用する。
    /// </summary>
    private void ApplyVolumes()
    {
        //BGMソースとSEソースがあるならそれぞれに音量を反映
        if (bgmSource != null) bgmSource.volume = bgmVolume;
        if (seSources != null)
            foreach (var src in seSources)
                src.volume = seVolume;
    }

    #endregion
}
