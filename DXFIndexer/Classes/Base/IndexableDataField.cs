using System;
using System.Linq;
using System.Reflection;
using DXFIndexer.Classes.Attributes;
using Sitecore.ContentSearch;

namespace DXFIndexer.Classes.Base
{
    public class IndexableDataField : IIndexableDataField
    {
        private object _concreteObject;
        private PropertyInfo _fieldInfo;

        public IndexableDataField(object concreteObject, PropertyInfo fieldInfo)
        {
            this._concreteObject = concreteObject;
            this._fieldInfo = fieldInfo;
        }

        public Type FieldType
        {
            get { return _fieldInfo.PropertyType; }
        }

        public object Id
        {
            get { return _fieldInfo.Name.ToLower(); }
        }

        public string Name
        {
            get { return _fieldInfo.Name; }
        }

        public string TypeKey
        {
            get { return string.Empty; }
        }

        public object Value
        {
            set { _fieldInfo.SetValue(_concreteObject, value); }
            get { return _fieldInfo.GetValue(_concreteObject); }
        }

        public bool IsIDField
        {
            get { return _fieldInfo.GetCustomAttributes(true).Any(attr => (attr as IndexableIdAttribute) != null); }
        }
    }
}
