using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private string lastSceneName = ""; // Daha önce yüklenen sahneyi saklamak için

    void Update()
    {
        // Android'den gelen Intent'i sürekli kontrol et
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject intent = currentActivity.Call<AndroidJavaObject>("getIntent");

        // Gönderilen "scene_name" verisini alın
        string sceneName = intent.Call<string>("getStringExtra", "scene_name");

        // Eğer yeni bir sahne adı geldiyse ve bu sahne yüklenmemişse
        if (!string.IsNullOrEmpty(sceneName) && sceneName != lastSceneName)
        {
            Debug.Log($"Yeni sahne yükleniyor: {sceneName}");
            lastSceneName = sceneName; // Yüklenen sahneyi kaydet
            SceneManager.LoadScene(sceneName);
        }
    }
}