using System;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
#endif

namespace TraversalPro
{
    [System.Serializable]
    public struct InterfaceRef<T> : ISerializationCallbackReceiver
        where T : class
    {
        [SerializeField] Object source;
        T _value;
        
        public T Value
        {
            get => _value;
            set => SetValue(value, out _value, out source);
        }

        public InterfaceRef(T value)
        {
            SetValue(value, out _value, out source);
        }

        public void OnBeforeSerialize()
        {
            if (source is GameObject go)
            {
                source = go.GetComponent<T>() as Object;
            }
            else if (source && source is not T)
            {
                source = null;
            }
        }

        public void OnAfterDeserialize()
        {
            _value = source as T;
        }

        static void SetValue(T value, out T _value, out Object source)
        {
            if (value is Object obj)
            {
                source = obj;
                _value = value;
            }
            else if (value == null)
            {
                source = null;
                _value = null;
            }
            else
            {
                throw new ArgumentException($"{value.GetType().Name} is not a UnityEngine.Object.");
            }
        }

        public static implicit operator T(InterfaceRef<T> value) => value.Value;
        public static implicit operator InterfaceRef<T>(T value) => new(value);
    }
     
    #if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(InterfaceRef<>))]
    public class RefEditor : PropertyDrawer  
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            PropertyField field = new(property.FindPropertyRelative("source"));
            field.label = $"{property.displayName}";
            field.tooltip = property.tooltip;
            field.RegisterValueChangeCallback(e =>
            {
                ObjectField objectField = field.Query<ObjectField>();
                if (objectField != null)
                {
                    if (!objectField.value)
                    {
                        Label label = objectField.Query<Label>(null, "unity-object-field-display__label");
                        if (label != null)
                        {
                            string typeName = fieldInfo.FieldType.GetGenericArguments()[0].Name;
                            label.text = $"None ({typeName})";
                        }
                    }
                }
            });
            // setting objectField.objectType to the interface type prevents dropping GameObjects
            return field;
        }
    }
    #endif
}