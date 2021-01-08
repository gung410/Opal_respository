using System;
using System.Collections.Generic;

namespace cxPlatform.Client.ConexusBase
{
    /// <summary>
    /// PaginatedList extension
    /// </summary>
    public static class PaginatedListExtension
    {
        /// <summary>
        /// Convert PaginatedList Entity to PaginatedList DTO
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="paginatedListEntity"></param>
        /// <param name="mapFromEntityToDtoFunction"></param>
        /// <returns></returns>
        public static PaginatedList<T> ToPaginatedListDto<E, T>(this PaginatedList<E> paginatedListEntity,
            Func<E, T> mapFromEntityToDtoFunction)
            where T : class
            where E : class
        {
            List<T> listDtos = new List<T>();
            foreach (var item in paginatedListEntity.Items)
            {
                var dto = mapFromEntityToDtoFunction(item);
                if (dto != null)
                {
                    listDtos.Add(dto);
                }
            }
            return new PaginatedList<T>(listDtos, paginatedListEntity.PageIndex, paginatedListEntity.PageSize, paginatedListEntity.HasMoreData) { TotalItems = paginatedListEntity.TotalItems }; ;
        }
        /// <summary>
        /// Convert PaginatedList Entity to PaginatedList DTO
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="D"></typeparam>
        /// <param name="getDynamicProperties"></param>
        /// <param name="paginatedListEntity"></param>
        /// <param name="mapFromEntityToDtoFunction"></param>
        /// <returns></returns>
        public static PaginatedList<D> ToPaginatedListDto<E, D>(this PaginatedList<E> paginatedListEntity,
            Func<E, bool?, D> mapFromEntityToDtoFunction, bool? getDynamicProperties = null)
            where D : class
            where E : class
        {
            List<D> listDtos = new List<D>();
            foreach (var item in paginatedListEntity.Items)
            {
                var dto = mapFromEntityToDtoFunction(item, getDynamicProperties);
                if (dto != null)
                {
                    listDtos.Add(dto);
                }
            }
            return new PaginatedList<D>(listDtos, paginatedListEntity.PageIndex, paginatedListEntity.PageSize, paginatedListEntity.HasMoreData) { TotalItems = paginatedListEntity.TotalItems };
        }
        /// <summary>
        /// Convert PaginatedList Entity to PaginatedList DTO
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="D"></typeparam>
        /// <param name="getDynamicProperties"></param>
        /// <param name="paginatedListEntity"></param>
        /// <param name="mapFromEntityToDtoFunction"></param>
        /// <returns></returns>
        public static PaginatedList<D> ToPaginatedListDto<E, UG, D>(this PaginatedList<E> paginatedListEntity,
            Func<E, bool?,List<UG>, D> mapFromEntityToDtoFunction, bool? getDynamicProperties = null, List<UG> parentUserGroupEntities = null)
            where D : class
            where E : class
        {
            List<D> listDtos = new List<D>();
            foreach (var item in paginatedListEntity.Items)
            {
                var dto = mapFromEntityToDtoFunction(item, getDynamicProperties, parentUserGroupEntities);
                if (dto != null)
                {
                    listDtos.Add(dto);
                }
            }
            return new PaginatedList<D>(listDtos, paginatedListEntity.PageIndex, paginatedListEntity.PageSize, paginatedListEntity.HasMoreData) { TotalItems = paginatedListEntity.TotalItems};
        }
        /// <summary>
        /// Convert PaginatedList Entity to PaginatedList DTO
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="paginatedListEntity"></param>
        /// <param name="mapFromEntityToDtoFunction"></param>
        /// <returns></returns>
        public static PaginatedList<T> ToPaginatedListDto<E, DTD, UG, T>(this PaginatedList<E> paginatedListEntity,
            Func<E, List<DTD>, List<UG>, T> mapFromEntityToDtoFunction, List<DTD> dtdEntities, List<UG> parentUserGroupEntitiesOfUsers)
            where T : class
            where E : class
        {
            List<T> listDtos = new List<T>();
            foreach (var item in paginatedListEntity.Items)
            {
                var dto = mapFromEntityToDtoFunction(item, dtdEntities, parentUserGroupEntitiesOfUsers);
                if (dto != null)
                {
                    listDtos.Add(dto);
                }
            }
            return new PaginatedList<T>(listDtos, paginatedListEntity.PageIndex, paginatedListEntity.PageSize, paginatedListEntity.HasMoreData) { TotalItems = paginatedListEntity.TotalItems }; ;
        }
    }
}
