using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using DXFIndexer.Classes.Attributes;
using Sitecore.ContentSearch;
using Sitecore.Data;
using Sitecore.Globalization;
using Version = Sitecore.Data.Version;

namespace DXFIndexer.Classes.Base
{
    public class BaseIndexable : IIndexable
    {
        private IEnumerable<IndexableDataField> _fields;
        private IIndexableId _id;
        private IIndexableUniqueId _uniqueId;

        private PropertyInfo[] _getProperties()
        {
            return this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        }

        public BaseIndexable()
        {
            _fields = _getProperties().Where(pr => pr.GetCustomAttributes(true).All(attr => (attr as ExcludeFromIndexAttribute) == null)).Select(fi => new IndexableDataField(this, fi));
        }

        public void LoadAllFields()
        {
            _fields = _getProperties().Where(pr => pr.GetCustomAttributes(true).All(attr => (attr as ExcludeFromIndexAttribute) == null)).Select(fi => new IndexableDataField(this, fi));
        }

        public IIndexableDataField GetFieldById(object fieldId)
        {
            return _fields.FirstOrDefault(f => f.Id == fieldId);
        }

        public IIndexableDataField GetFieldByName(string fieldName)
        {
            return _fields.FirstOrDefault(f => f.Name.ToLower() == fieldName.ToLower());
        }

        [ExcludeFromIndex]
        public IIndexableId Id
        {
            get
            {
                if (_id == null)
                {
                    var idField = _fields.FirstOrDefault(fi => fi.IsIDField);
                    _id = new IndexableId<string>(idField.Value.ToString());
                }
                return _id;
            }
        }

        /// <summary>
        /// Mocks the Unique ID produced by Sitecore, so sitecore://{DataSource}/{MD5(ID)}?lang={Culture}&ver=0
        /// </summary>
        [ExcludeFromIndex]
        public virtual IIndexableUniqueId UniqueId
        {
            get
            {
                var hashedId = CreateHash(Id.Value.ToString());
                return (IIndexableUniqueId)(SitecoreItemUniqueId) new ItemUri(hashedId, Language.Parse(Culture.Name), Version.Latest, DataSource);
            }
        }

        /// <summary>
        /// Datasource is the type of the class for transparencies sake
        /// </summary>
        public string DataSource
        {
            get { return this.GetType().Name; }
        }

        [ExcludeFromIndex]
        public string AbsolutePath
        {
            get { return "/"; }
        }
        [ExcludeFromIndex]
        public CultureInfo Culture
        {
            get { return new CultureInfo("en"); }
        }
        [ExcludeFromIndex]
        public IEnumerable<IIndexableDataField> Fields
        {
            get { return _fields; }
        }

        [ExcludeFromIndex]
        public ID CreateHash(string id)
        {
            MD5 md5Hasher = MD5.Create();
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(Id.Value.ToString()));
            var hashedId = new ID(new Guid(data));
            return hashedId;
        }
    }
}
