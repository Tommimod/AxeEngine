#if UNITY_EDITOR && AXE_ENGINE_ENABLE_STATIC
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AxeEngine.Editor
{
    public class ActorHistoryEditorWindow : EditorWindow
    {
        private VisualTreeAsset _window;
        private VisualTreeAsset _actorEvent;

        private bool _showAdded;
        private bool _showReplaced;
        private bool _showRemoved;

        private long _i;

        [MenuItem("Window/Actor History")]
        public static void ShowActorHistory()
        {
            var wnd = GetWindow<ActorHistoryEditorWindow>();
            wnd.titleContent = new GUIContent("Actor History");
        }

        public void CreateGUI()
        {
            if (Application.isPlaying)
            {
                Debug.Break();
            }

            var allAssetPaths = AssetDatabase.GetAllAssetPaths();
            var windowPath = allAssetPaths.FirstOrDefault(x => x.EndsWith("Actor History.uxml"));
            var actorEventPath = allAssetPaths.FirstOrDefault(x => x.EndsWith("ActorEventTemplete.uxml"));
            _window = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(windowPath);
            _actorEvent = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(actorEventPath);

            var windowView = _window.Instantiate();
            rootVisualElement.Add(windowView);

            var showAdded = windowView.Q<Toggle>("Added");
            _showAdded = showAdded.value;
            showAdded.RegisterCallback<ChangeEvent<bool>>(IsAdded);
            var showReplaced = windowView.Q<Toggle>("Replaced");
            _showReplaced = showReplaced.value;
            showReplaced.RegisterCallback<ChangeEvent<bool>>(IsReplaced);
            var showRemoved = windowView.Q<Toggle>("Removed");
            _showRemoved = showRemoved.value;
            showRemoved.RegisterCallback<ChangeEvent<bool>>(IsRemoved);

            WorldEditorEventCollector.OnActorEvent += AddEvent;
            Redraw();
        }

        private void OnDestroy()
        {
            WorldEditorEventCollector.OnActorEvent -= AddEvent;
            var showAdded = rootVisualElement.Q<Toggle>("Added");
            showAdded.UnregisterCallback<ChangeEvent<bool>>(IsAdded);
            var showReplaced = rootVisualElement.Q<Toggle>("Replaced");
            showReplaced.UnregisterCallback<ChangeEvent<bool>>(IsReplaced);
            var showRemoved = rootVisualElement.Q<Toggle>("Removed");
            showRemoved.UnregisterCallback<ChangeEvent<bool>>(IsRemoved);
        }

        private void IsRemoved(ChangeEvent<bool> evt)
        {
            _showRemoved = evt.newValue;
            Redraw();
        }

        private void IsReplaced(ChangeEvent<bool> evt)
        {
            _showReplaced = evt.newValue;
            Redraw();
        }

        private void IsAdded(ChangeEvent<bool> evt)
        {
            _showAdded = evt.newValue;
            Redraw();
        }

        private void Redraw()
        {
            _i = 0;
            var scrollView = rootVisualElement.Q<ScrollView>("ScrollView");
            scrollView.contentContainer.Clear();

            var events = WorldEditorEventCollector.ActorEvents;
            foreach (var entityEvent in events)
            {
                AddEvent(entityEvent);
            }
        }

        private void AddEvent(ActorEvent data)
        {
            switch (data.EventType)
            {
                case ActorEvent.Type.Added when !_showAdded:
                case ActorEvent.Type.Replaced when !_showReplaced:
                case ActorEvent.Type.Removed when !_showRemoved:
                    return;
            }

            var scrollView = rootVisualElement.Q<ScrollView>("ScrollView");
            var entityEvent = _actorEvent.Instantiate();
            entityEvent.Q<Label>("Index").text = _i++.ToString();
            entityEvent.Q<Label>("EventType").text = data.EventType.ToString();
            entityEvent.Q<Label>("ComponentType").text = data.PropertyName;
            entityEvent.Q<Label>("ComponentValue").text = data.PropertyValue;
            entityEvent.Q<Label>("GOName").text = data.GameObjectName;
            entityEvent.Q<Label>("Time").text = data.Time;
            scrollView.contentContainer.Add(entityEvent);
            entityEvent.RegisterCallback<ClickEvent>(_ => OpenData(data));
        }

        private static void OpenData(ActorEvent data)
        {
            Debug.Log(data.StackTrace);
        }
    }
}
#endif