using UnityEngine;
using UnityEngine.SceneManagement;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "New Scene Descriptor", menuName = "Scene Descriptor")]
    public class SceneDescriptor : ScriptableObject
    {
        [SceneName]
        public string Scene;
    }
}