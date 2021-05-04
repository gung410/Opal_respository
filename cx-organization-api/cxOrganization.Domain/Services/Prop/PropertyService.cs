using System;
using System.Collections.Generic;
using System.Linq;
using cxOrganization.Domain.AdvancedWorkContext;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Repositories;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;

namespace cxOrganization.Domain.Services
{
    public class PropertyService : IPropertyService
    {
        private readonly IPropValueRepository _propValueRepository;
        private readonly IPropOptionRepository _propOptionRepository;
        private readonly IPropPageRepository _propPageRepository;
        private readonly IAdvancedWorkContext _workContext;
        private readonly IPropertyRepository _propertyRepository;

        public PropertyService(IPropPageRepository propPageRepository,
            IPropOptionRepository propOptionRepository,
            IPropValueRepository propValueRepository,
            IPropertyRepository propertyRepository,
            IAdvancedWorkContext workContext)
        {
            _propValueRepository = propValueRepository;
            _propOptionRepository = propOptionRepository;
            _propPageRepository = propPageRepository;
            _propertyRepository = propertyRepository;
            _workContext = workContext;
        }
        public List<EntityKeyValueDto> GetDynamicProperties(int itemId, TableTypes tableType)
        {
            var result = new List<EntityKeyValueDto>();
            var tableTypePropPage = GetPropPagesByTableType(tableType);
            foreach (var item in tableTypePropPage)
            {
                foreach (var prop in item.Properties)
                {
                    if (prop.MultiValue)
                    {
                        var propvalues = _propValueRepository.GetPropValuesByItemId(itemId, prop.PropertyId);
                        foreach (var propvalue in propvalues)
                        {
                            var dynamicProperty = GetDynamicProperty(propvalue);
                            if (dynamicProperty != null)
                                result.Add(dynamicProperty);
                        }
                    }
                    else
                    {
                        var propData = FindPropValueByItemIdAndPropertyId(itemId, prop.PropertyId, false);
                        var dynamicProperty = GetDynamicProperty(propData);
                        if (dynamicProperty != null)
                            result.Add(dynamicProperty);
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// Finds the prop value.
        /// </summary>
        /// <param name="itemId">The item id.</param>
        /// <param name="propertyId">The property id.</param>
        /// <param name="putToCache">decide put to cache or not</param>
        /// <returns>PropValue.</returns>
        public PropValueEntity FindPropValueByItemIdAndPropertyId(int itemId, int propertyId, bool putToCache = true)
        {
            if (putToCache)
            {
                var propValue = _propValueRepository.FindPropValueByItemIdAndPropertyId(itemId, propertyId);
                return propValue;
            }
            return _propValueRepository.FindPropValueByItemIdAndPropertyId(itemId, propertyId);
        }
        public Dictionary<int, List<EntityKeyValueDto>> GetDynamicProperties(List<int> itemIds, TableTypes tableType)
        {
            var result = new Dictionary<int, List<EntityKeyValueDto>>();
            var tableTypePropPage = GetPropPagesByTableType(tableType);
            var propertyIds = new List<int>();
            foreach (var item in tableTypePropPage)
            {
                propertyIds.AddRange(item.Properties.Select(p => p.PropertyId).ToList());
            }
            var propvalues = _propValueRepository.GetPropValuesByItemIds(itemIds, propertyIds);
            foreach (var item in propvalues.GroupBy(p => p.ItemId))
            {
                var dynamicProperties = new List<EntityKeyValueDto>();
                foreach (var propvalue in item.ToList())
                {
                    var dynamicProperty = GetDynamicProperty(propvalue);
                    dynamicProperties.Add(dynamicProperty);
                }
                result.Add(item.Key, dynamicProperties);
            }
            return result;
        }

        private EntityKeyValueDto GetDynamicProperty(PropValueEntity propvalue)
        {
            if (propvalue == null)
                return null;
            if (propvalue.PropOptionId.HasValue)
            {
                var propOption = _propOptionRepository.GetPropOptionById(propvalue.PropOptionId.Value);
                if (propOption != null)
                {
                    return new EntityKeyValueDto
                    {
                        Key = GetPropertyName(propvalue.PropertyId, _workContext.CurrentLanguageId),
                        Value = new Func<string>(() =>
                        {
                            var lt = propOption.LtPropOptions.FirstOrDefault(y => y.LanguageId == _workContext.CurrentLanguageId);
                            return lt != null ? lt.Name : string.Empty;
                        })()
                    };
                }

            }
            else
            {
                //skip getting propfile
                if (!propvalue.PropFileId.HasValue)
                {
                    return new EntityKeyValueDto
                    {
                        Key = GetPropertyName(propvalue.PropertyId, _workContext.CurrentLanguageId),
                        Value = propvalue.Value
                    };
                }
                else
                {
                    //Todo: not sure what to do with this
                    return null;
                }
            }
            return null;
        }

        public string GetPropertyName(int propertyId, int languageId)
        {
            var prop = _propertyRepository.GetProperties().FirstOrDefault(x => x.PropertyId == propertyId);
            if (prop != null)
            {
                var lt = prop.LtProperties.FirstOrDefault(x => x.LanguageId == languageId);
                if (lt != null)
                    return lt.Name;
                return string.Empty;
            }
            return string.Empty;
        }

        private List<PropPageEntity> GetPropPagesByTableType(TableTypes tableType)
        {
            var listPropPage = _propPageRepository.GetPropPagesByTableTypeId((short)tableType);

            return listPropPage;
        }

        /// <summary>
        /// Finds the prop value include prop.
        /// </summary>
        /// <param name="itemId">The item id.</param>
        /// <param name="propertyId">The property id.</param>
        /// <returns>PropValue.</returns>
        public PropValueEntity FindPropValueIncludeProp(int itemId, int propertyId)
        {
            var propValue = _propValueRepository.FindPropValueIncludeProp(itemId, propertyId);

            return propValue;
        }
    }
}
