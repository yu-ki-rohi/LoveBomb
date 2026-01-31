// yu-ki-rohi
// 参考サイト
// https://graphicalpoxy.hatenablog.com/entry/2021/06/02/113447

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Video;

public class MovieSceneManager : MonoBehaviour
{
   
    [SerializeField] private VideoPlayer _videoPlayer;
    [SerializeField] private Image _backGround;
   

    private void OnPrepareCompleted(VideoPlayer vp)
    {
        SwitchBackGround();
    }

    private void OnLoopPointReached(VideoPlayer vp)
    {
        ChangeScene();
    }

    private void ChangeScene()
    {
        SwitchBackGround();
        SceneTransitionManager.Instance.TransitionToNextScene();
    }
    private bool SwitchBackGround()
    {
        _backGround.enabled = !_backGround.enabled;
        return _backGround.enabled;
    }
    // Start is called before the first frame update
    void Start()
    {
        _videoPlayer.loopPointReached += OnLoopPointReached;
        _videoPlayer.prepareCompleted += OnPrepareCompleted;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            ChangeScene();
        }
    }

    private void OnDestroy()
    {
        _videoPlayer.loopPointReached -= OnLoopPointReached;
        _videoPlayer.prepareCompleted -= OnPrepareCompleted;
    }
    
   
}
