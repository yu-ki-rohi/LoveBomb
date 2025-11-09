// 参考資料：https://qiita.com/KeichiMizutani/items/ca46a40de02e87b3d8a8

using UnityEngine.Pool;

// yu-ki-rohi
// interfaceが何かわからない人は「C# インターフェース」で検索してください

// yu-ki-rohi
// Tが何か分からない人はジェネリックで検索してください
// ちなみにC++でいうところのテンプレートだと思います

// yu-kirohi
// オブジェクトプールで管理するオブジェクトに継承してください
// 例) public class SomeObject : MonoBehavior, IPooledObject<SomeObject>
public interface IPooledObject<T> where T : class
{
    public IObjectPool<T> ObjectPool { set; }
    public void Initialize();
    public void Deactivate();
}
