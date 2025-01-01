using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace AxeEngine.Editor.Toolkit
{
    [CustomEditor(typeof(AxeEditorActor))]
    public class AxeInspectorToolkit : UnityEditor.Editor
    {
        private VisualTreeAsset _inspectorTreeAsset;
        private VisualTreeAsset _componentTreeAsset;
        private VisualTreeAsset _fieldInfoAsset;
        private VisualElement _myInspector;
        private ListView _componentList;
        private Button _addButton;
        private DropdownField _componentsDropdown;
        private AxeEditorActor _editorActor;

        private List<Type> _allTypes;
        private int _selectedComponentIndex;
        private readonly Dictionary<VisualElement, Array> _elementToArray = new();

        public override VisualElement CreateInspectorGUI()
        {
            _inspectorTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/AxeEngineRepo/Runtime/Unity/Editor/Toolkit/AxeInspectorToolkit.uxml");
            _componentTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/AxeEngineRepo/Runtime/Unity/Editor/Toolkit/AxeComponentToolkit.uxml");
            _fieldInfoAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/AxeEngineRepo/Runtime/Unity/Editor/Toolkit/AxeComponentFieldInfoToolkit.uxml");
            _editorActor = (AxeEditorActor)target;
            _allTypes = AxeReflection.GetAllAxeTypes();

            _myInspector = new VisualElement();
            _inspectorTreeAsset.CloneTree(_myInspector);

            _componentList = _myInspector.Q<ListView>("ComponentList");
            _addButton = _myInspector.Q<Button>("AddButton");
            _componentsDropdown = _myInspector.Q<DropdownField>("ComponentsDropdown");

            HandleComponentDropdown();
            HandleAddButton(_componentsDropdown.value);
            HandleComponentList();
            _addButton.clicked += OnAddButtonClicked;
            return _myInspector;
        }

        private void HandleComponentDropdown()
        {
            for (var i = 0; i < _allTypes.Count; i++)
            {
                var type = _allTypes[i];
                _componentsDropdown.choices.Add($"{type.GetCustomAttribute<AxeProperty>().Path}{type.Name}");
            }

            _componentsDropdown.value ??= _componentsDropdown.choices[0] ?? string.Empty;
            _componentsDropdown.RegisterCallback<ChangeEvent<string>>(OnComponentSelected);
        }

        private void OnComponentSelected(ChangeEvent<string> evt)
        {
            HandleAddButton(evt.newValue);
        }

        private void HandleAddButton(string componentName)
        {
            _selectedComponentIndex = Array.IndexOf(_componentsDropdown.choices.ToArray(), componentName);
            if (_selectedComponentIndex < 0)
            {
                _addButton.SetEnabled(false);
                return;
            }

            var selectedComponentType = _allTypes[_selectedComponentIndex];
            var isAdded = _editorActor.Properties.Any(x => x.GetType() == selectedComponentType);
            _addButton.SetEnabled(!isAdded);
        }

        private void OnAddButtonClicked()
        {
            var componentInstance = Activator.CreateInstance(_allTypes[_selectedComponentIndex]);
            _editorActor.Properties.Add(componentInstance);
            _editorActor.Validate();

            _addButton.SetEnabled(false);
            HandleComponentList();
            EditorUtility.SetDirty(target);
        }

        private void HandleComponentList()
        {
            _componentList.hierarchy.Clear();
            foreach (var component in _editorActor.Properties)
            {
                var componentVisualElement = new VisualElement();
                _componentTreeAsset.CloneTree(componentVisualElement);
                _componentList.hierarchy.Add(componentVisualElement);

                var label = componentVisualElement.Q<Label>("ComponentType");
                var componentName = component.ToString();
                label.text = GetNameAfterLastDot(componentName);

                componentVisualElement.Q<Button>("Remove").clicked += () =>
                {
                    _editorActor.Properties.Remove(component);
                    _editorActor.Validate();
                    componentVisualElement.RemoveFromHierarchy();
                    HandleAddButton(_componentsDropdown.value);
                    EditorUtility.SetDirty(target);
                };

                var currentType = component.GetType();
                var fieldsInfo = currentType.GetAllFields();

                foreach (var fieldInfo in fieldsInfo)
                {
                    VisualElement fieldContainer = new VisualElement();
                    componentVisualElement.hierarchy[0].hierarchy[1].hierarchy.Add(fieldContainer);
                    _fieldInfoAsset.CloneTree(fieldContainer);
                    fieldContainer.style.width = Length.Percent(100);
                    var fieldName = fieldContainer.Q<Label>("FieldName");
                    if (fieldsInfo.Length == 0)
                    {
                        fieldName.RemoveFromHierarchy();
                        break;
                    }

                    var customInfo = new AxeFieldInfo(fieldInfo.Name, fieldInfo.FieldType, fieldInfo.GetValue(component));
                    CreateField(customInfo, fieldContainer, component, fieldName);
                }
            }
        }

        private void SetValueToActor(object component, AxeFieldInfo fieldInfo, object value)
        {
            var componentInstance = _editorActor.Properties.FirstOrDefault(x => x == component);
            componentInstance?.GetType().GetField(fieldInfo.Name).SetValue(componentInstance, value);
            EditorUtility.SetDirty(target);
        }

        private VisualElement CreateField(AxeFieldInfo fieldInfo, VisualElement fieldContainer, object component, Label fieldName = null)
        {
            if (fieldName != null)
            {
                fieldName.text = fieldInfo.Name;
            }

            VisualElement visualElement = null;
            var fieldType = fieldInfo.FieldType;
            if (fieldType.IsInteger())
            {
                var value = (int)fieldInfo.Value;
                visualElement = new IntegerField { value = value };
                if (fieldContainer.hierarchy.childCount == 0)
                {
                    fieldContainer.contentContainer.Add(visualElement);
                }
                else
                {
                    if (fieldName == null)
                    {
                        fieldContainer.hierarchy.Add(visualElement);
                    }
                    else
                    {
                        fieldContainer.hierarchy[0].Add(visualElement);
                    }
                }

                visualElement.RegisterCallback<ChangeEvent<int>>(OnChanged);

                void OnChanged(ChangeEvent<int> evt)
                {
                    SetValueToActor(component, fieldInfo, evt.newValue);
                }
            }
            else if (fieldType.IsFloat())
            {
                var value = (float)fieldInfo.Value;
                visualElement = new FloatField { value = value };
                if (fieldContainer.hierarchy.childCount == 0)
                {
                    fieldContainer.contentContainer.Add(visualElement);
                }
                else
                {
                    if (fieldName == null)
                    {
                        fieldContainer.hierarchy.Add(visualElement);
                    }
                    else
                    {
                        fieldContainer.hierarchy[0].Add(visualElement);
                    }
                }

                visualElement.RegisterCallback<ChangeEvent<float>>(OnChanged);

                void OnChanged(ChangeEvent<float> evt)
                {
                    SetValueToActor(component, fieldInfo, evt.newValue);
                }
            }
            else if (fieldType.IsBool())
            {
                var value = (bool)fieldInfo.Value;
                visualElement = new Toggle { value = value };
                if (fieldContainer.hierarchy.childCount == 0)
                {
                    fieldContainer.contentContainer.Add(visualElement);
                }
                else
                {
                    if (fieldName == null)
                    {
                        fieldContainer.hierarchy.Add(visualElement);
                    }
                    else
                    {
                        fieldContainer.hierarchy[0].Add(visualElement);
                    }
                }

                visualElement.RegisterCallback<ChangeEvent<bool>>(OnChanged);

                void OnChanged(ChangeEvent<bool> evt)
                {
                    SetValueToActor(component, fieldInfo, evt.newValue);
                }
            }
            else if (fieldType.IsString())
            {
                var value = (string)fieldInfo.Value;
                visualElement = new TextField { value = value };
                if (fieldContainer.hierarchy.childCount == 0)
                {
                    fieldContainer.contentContainer.Add(visualElement);
                }
                else
                {
                    if (fieldName == null)
                    {
                        fieldContainer.hierarchy.Add(visualElement);
                    }
                    else
                    {
                        fieldContainer.hierarchy[0].Add(visualElement);
                    }
                }

                visualElement.RegisterCallback<ChangeEvent<string>>(OnChanged);

                void OnChanged(ChangeEvent<string> evt)
                {
                    SetValueToActor(component, fieldInfo, evt.newValue);
                }
            }
            else if (fieldType.IsEnum)
            {
                var value = (Enum)fieldInfo.Value;
                var enumDropdown = new EnumField { value = value };
                enumDropdown.Init(value);
                visualElement = enumDropdown;
                if (fieldContainer.hierarchy.childCount == 0)
                {
                    fieldContainer.contentContainer.Add(visualElement);
                }
                else
                {
                    if (fieldName == null)
                    {
                        fieldContainer.hierarchy.Add(visualElement);
                    }
                    else
                    {
                        fieldContainer.hierarchy[0].Add(visualElement);
                    }
                }

                visualElement.RegisterCallback<ChangeEvent<Enum>>(OnChanged);

                void OnChanged(ChangeEvent<Enum> evt)
                {
                    SetValueToActor(component, fieldInfo, evt.newValue);
                }
            }
            else if (fieldType.IsUnityObject())
            {
                var value = (UnityEngine.Object)fieldInfo.Value;
                visualElement = new ObjectField { value = value };
                if (fieldContainer.hierarchy.childCount == 0)
                {
                    fieldContainer.contentContainer.Add(visualElement);
                }
                else
                {
                    if (fieldName == null)
                    {
                        fieldContainer.hierarchy.Add(visualElement);
                    }
                    else
                    {
                        fieldContainer.hierarchy[0].Add(visualElement);
                    }
                }

                visualElement.RegisterCallback<ChangeEvent<UnityEngine.Object>>(OnChanged);
                var objectField = (ObjectField)visualElement;
                objectField.objectType = fieldType;
                objectField.allowSceneObjects = true;

                void OnChanged(ChangeEvent<UnityEngine.Object> evt)
                {
                    SetValueToActor(component, fieldInfo, evt.newValue);
                }
            }
            else if (fieldType.IsVector2())
            {
                var value = (Vector2)fieldInfo.Value;
                visualElement = new Vector2Field { value = value };
                if (fieldContainer.hierarchy.childCount == 0)
                {
                    fieldContainer.contentContainer.Add(visualElement);
                }
                else
                {
                    if (fieldName == null)
                    {
                        fieldContainer.hierarchy.Add(visualElement);
                    }
                    else
                    {
                        fieldContainer.hierarchy[0].Add(visualElement);
                    }
                }

                visualElement.RegisterCallback<ChangeEvent<Vector2>>(OnChanged);

                void OnChanged(ChangeEvent<Vector2> evt)
                {
                    SetValueToActor(component, fieldInfo, evt.newValue);
                }
            }
            else if (fieldType.IsVector3())
            {
                var value = (Vector3)fieldInfo.Value;
                visualElement = new Vector3Field { value = value };
                if (fieldContainer.hierarchy.childCount == 0)
                {
                    fieldContainer.contentContainer.Add(visualElement);
                }
                else
                {
                    if (fieldName == null)
                    {
                        fieldContainer.hierarchy.Add(visualElement);
                    }
                    else
                    {
                        fieldContainer.hierarchy[0].Add(visualElement);
                    }
                }

                visualElement.RegisterCallback<ChangeEvent<Vector3>>(OnChanged);

                void OnChanged(ChangeEvent<Vector3> evt)
                {
                    SetValueToActor(component, fieldInfo, evt.newValue);
                }
            }
            else if (fieldType.IsVector4())
            {
                var value = (Vector4)fieldInfo.Value;
                visualElement = new Vector4Field { value = value };
                if (fieldContainer.hierarchy.childCount == 0)
                {
                    fieldContainer.contentContainer.Add(visualElement);
                }
                else
                {
                    if (fieldName == null)
                    {
                        fieldContainer.hierarchy.Add(visualElement);
                    }
                    else
                    {
                        fieldContainer.hierarchy[0].Add(visualElement);
                    }
                }

                visualElement.RegisterCallback<ChangeEvent<Vector4>>(OnChanged);

                void OnChanged(ChangeEvent<Vector4> evt)
                {
                    SetValueToActor(component, fieldInfo, evt.newValue);
                }
            }
            else if (fieldType.IsColor())
            {
                var value = (Color)fieldInfo.Value;
                visualElement = new ColorField { value = value };
                if (fieldContainer.hierarchy.childCount == 0)
                {
                    fieldContainer.contentContainer.Add(visualElement);
                }
                else
                {
                    if (fieldName == null)
                    {
                        fieldContainer.hierarchy.Add(visualElement);
                    }
                    else
                    {
                        fieldContainer.hierarchy[0].Add(visualElement);
                    }
                }

                visualElement.RegisterCallback<ChangeEvent<Color>>(OnChanged);

                void OnChanged(ChangeEvent<Color> evt)
                {
                    SetValueToActor(component, fieldInfo, evt.newValue);
                }
            }
            else if (fieldType.IsCurve())
            {
                var value = (AnimationCurve)fieldInfo.Value;
                visualElement = new CurveField { value = value };
                if (fieldContainer.hierarchy.childCount == 0)
                {
                    fieldContainer.contentContainer.Add(visualElement);
                }
                else
                {
                    if (fieldName == null)
                    {
                        fieldContainer.hierarchy.Add(visualElement);
                    }
                    else
                    {
                        fieldContainer.hierarchy[0].Add(visualElement);
                    }
                }

                visualElement.RegisterCallback<ChangeEvent<AnimationCurve>>(OnChanged);

                void OnChanged(ChangeEvent<AnimationCurve> evt)
                {
                    SetValueToActor(component, fieldInfo, evt.newValue);
                }
            }
            else if (fieldType.IsArray())
            {
                var isList = false;
                var elementType = fieldType.GetElementType()!;
                if (elementType == null)
                {
                    isList = true;
                    elementType = fieldType.GenericTypeArguments[0];
                }

                if (!elementType.IsSupportedType())
                {
                    return null;
                }

                var instance = fieldInfo.Value as Array;
                if (isList)
                {
                    var list = fieldInfo.Value as IList;
                    list ??= new List<object>();
                    var count = list.Count;
                    instance = Array.CreateInstance(elementType, count);
                    for (var i = 0; i < count; i++)
                    {
                        instance.SetValue(list[i], i);
                    }
                }

                instance ??= Array.CreateInstance(elementType, 0);
                visualElement = new Foldout { value = true };
                if (fieldContainer.hierarchy.childCount == 0)
                {
                    fieldContainer.contentContainer.Add(visualElement);
                }
                else
                {
                    if (fieldName == null)
                    {
                        fieldContainer.hierarchy.Add(visualElement);
                    }
                    else
                    {
                        fieldContainer.hierarchy[0].Add(visualElement);
                    }
                }

                var plusButton = new Button
                {
                    text = "+",
                    style =
                    {
                        height = new StyleLength(new Length(20, LengthUnit.Pixel))
                    }
                };
                fieldContainer.Add(plusButton);
                plusButton.clicked += OnPlusButtonClicked;
                PlusButtonClicked(false);

                void OnPlusButtonClicked() => PlusButtonClicked();

                void PlusButtonClicked(bool save = true)
                {
                    if (instance != null)
                    {
                        var resize = save ? instance.Length + 1 : instance.Length;
                        if (save)
                        {
                            var instanceBeforeAdd = instance;
                            instance = Array.CreateInstance(elementType, resize);
                            if (instanceBeforeAdd.Length != 0)
                            {
                                for (var i = 0; i < instanceBeforeAdd.Length; i++)
                                {
                                    instance.SetValue(instanceBeforeAdd.GetValue(i), i);
                                }
                            }

                            Save();
                        }

                        while (instance.Length > 0 && visualElement.contentContainer.childCount < resize)
                        {
                            var elementId = visualElement.contentContainer.childCount;
                            var currentArrayField = instance.GetValue(elementId);
                            var customInfo = new AxeFieldInfo($"{elementId}", elementType, currentArrayField);
                            var arrayVisualElement = CreateField(customInfo, visualElement.contentContainer, currentArrayField);
                            var minusButton = new Button
                            {
                                text = "-",
                                style =
                                {
                                    height = new StyleLength(new Length(20, LengthUnit.Pixel))
                                }
                            };
                            arrayVisualElement.Add(minusButton);
                            minusButton.clicked += () => MinusButtonClicked(elementId, arrayVisualElement);
                            SubscribeInnerFieldOnChangeEvent(arrayVisualElement, elementId, fieldInfo, Save);
                        }

                        for (var i = 0; i < visualElement.contentContainer.childCount; i++)
                        {
                            var arrayVisualElement = visualElement.contentContainer[i];
                            _elementToArray[arrayVisualElement] = instance;
                        }

                        return;

                        void Save()
                        {
                            IList list = null;
                            if (isList)
                            {
                                list = Activator.CreateInstance(fieldType) as IList;
                                foreach (var data in instance)
                                {
                                    list.Add(data);
                                }
                            }

                            SetValueToActor(component, fieldInfo, isList ? list : instance);
                        }

                        void MinusButtonClicked(int elementId, VisualElement arrayVisualElement)
                        {
                            var lessSize = instance.Length - 1;
                            var instanceBeforeRemove = instance;
                            instance = Array.CreateInstance(elementType, lessSize);
                            if (instance.Length != 0)
                            {
                                var offset = 0;
                                for (var i = 0; i < instance.Length; i++)
                                {
                                    if (i == elementId)
                                    {
                                        offset++;
                                    }

                                    instance.SetValue(instanceBeforeRemove.GetValue(i + offset), i);
                                }
                            }

                            Save();
                            arrayVisualElement.RemoveFromHierarchy();
                        }
                    }
                }
            }

            if (visualElement == null)
            {
                return null;
            }

            visualElement.style.width = fieldName == null ? Length.Percent(95) : Length.Percent(70);
            return visualElement;
        }

        private void SubscribeInnerFieldOnChangeEvent(VisualElement element, int elementIndex, AxeFieldInfo fieldInfo, Action onChanged = null)
        {
            var type = fieldInfo.FieldType.GetElementType();
            type ??= fieldInfo.FieldType.GenericTypeArguments[0];

            if (type.IsBool())
            {
                element.RegisterCallback<ChangeEvent<bool>>(OnChanged);

                void OnChanged(ChangeEvent<bool> evt)
                {
                    _elementToArray[element].SetValue(evt.newValue, elementIndex);
                    onChanged?.Invoke();
                }
            }
            else if (type.IsInteger())
            {
                element.RegisterCallback<ChangeEvent<int>>(OnChanged);

                void OnChanged(ChangeEvent<int> evt)
                {
                    _elementToArray[element].SetValue(evt.newValue, elementIndex);
                    onChanged?.Invoke();
                }
            }
            else if (type.IsFloat())
            {
                element.RegisterCallback<ChangeEvent<float>>(OnChanged);

                void OnChanged(ChangeEvent<float> evt)
                {
                    _elementToArray[element].SetValue(evt.newValue, elementIndex);
                    onChanged?.Invoke();
                }
            }
            else if (type.IsString())
            {
                element.RegisterCallback<ChangeEvent<string>>(OnChanged);

                void OnChanged(ChangeEvent<string> evt)
                {
                    _elementToArray[element].SetValue(evt.newValue, elementIndex);
                    onChanged?.Invoke();
                }
            }
            else if (type.IsUnityObject())
            {
                element.RegisterCallback<ChangeEvent<UnityEngine.Object>>(OnChanged);

                void OnChanged(ChangeEvent<UnityEngine.Object> evt)
                {
                    _elementToArray[element].SetValue(evt.newValue, elementIndex);
                    onChanged?.Invoke();
                }
            }
            else if (type.IsColor())
            {
                element.RegisterCallback<ChangeEvent<Color>>(OnChanged);

                void OnChanged(ChangeEvent<Color> evt)
                {
                    _elementToArray[element].SetValue(evt.newValue, elementIndex);
                    onChanged?.Invoke();
                }
            }
            else if (type.IsCurve())
            {
                element.RegisterCallback<ChangeEvent<AnimationCurve>>(OnChanged);

                void OnChanged(ChangeEvent<AnimationCurve> evt)
                {
                    _elementToArray[element].SetValue(evt.newValue, elementIndex);
                    onChanged?.Invoke();
                }
            }
            else if (type.IsVector2())
            {
                element.RegisterCallback<ChangeEvent<Vector2>>(OnChanged);

                void OnChanged(ChangeEvent<Vector2> evt)
                {
                    _elementToArray[element].SetValue(evt.newValue, elementIndex);
                    onChanged?.Invoke();
                }
            }
            else if (type.IsVector3())
            {
                element.RegisterCallback<ChangeEvent<Vector3>>(OnChanged);

                void OnChanged(ChangeEvent<Vector3> evt)
                {
                    _elementToArray[element].SetValue(evt.newValue, elementIndex);
                    onChanged?.Invoke();
                }
            }
            else if (type.IsVector4())
            {
                element.RegisterCallback<ChangeEvent<Vector4>>(OnChanged);

                void OnChanged(ChangeEvent<Vector4> evt)
                {
                    _elementToArray[element].SetValue(evt.newValue, elementIndex);
                    onChanged?.Invoke();
                }
            }
        }

        private static string GetNameAfterLastDot(string nameOfType)
        {
            return nameOfType.Split('.').Last();
        }
    }
}