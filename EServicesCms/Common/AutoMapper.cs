using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Web;

namespace EServicesCms.Common
{
    public class HeckAttribute : Attribute
    {

    }

    public class IgnoreMap : Attribute
    {

    }
    public static class AutoMapper
    {

        /// <summary>
        /// Generic Mapper for DTO to Domain Entity
        /// This method will create destination object and its all heirarchy.
        /// it will work over naming convention of properties and entities
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="Source"></param>
        /// <param name="destination"></param>
        public static U LoadEntityFromDto<T, U>(T Source)
        //where T : BaseDTO
        //where U : BaseEntity
        {
            U destination = Activator.CreateInstance<U>();
            foreach (PropertyInfo sourceProperty in Source.GetType().GetProperties())
            {
                //MappingAttribute mappingAttribute = Utility.GetAutoMappingAttribute(sourceProperty);
                //if (mappingAttribute != null)
                //    continue;
                if (sourceProperty.GetCustomAttributes(typeof(HeckAttribute)).Count() > 0)
                    continue;
                if (sourceProperty.GetCustomAttributes(typeof(IgnoreMap)).Count() > 0)
                    continue;
                PropertyInfo destinationProperty = destination.GetType().GetProperty(sourceProperty.Name);
                if (destinationProperty != null && sourceProperty.GetValue(Source) != null && destinationProperty.SetMethod != null)
                {
                    //For primitive types assign values directly
                    if (sourceProperty.PropertyType.IsValueType || sourceProperty.PropertyType == typeof(System.String))
                    {
                        destinationProperty.SetValue(destination, sourceProperty.GetValue(Source));
                    }
                    //If property is type of composite object, call method recursively using reflection
                    else if (sourceProperty.PropertyType.IsSubclassOf(typeof(T)) || sourceProperty.PropertyType.Namespace.Contains("SDTF"))
                    {
                        var nestedObject = sourceProperty.GetValue(Source);
                        Type typeSource = nestedObject.GetType();
                        Type typeDestination = destinationProperty.PropertyType;
                        MethodInfo method = typeof(AutoMapper).GetMethod("LoadEntityFromDto")
                             .MakeGenericMethod(new Type[] { typeSource, typeDestination });
                        var retValue = method.Invoke(null, new object[] { nestedObject });
                        destinationProperty.SetValue(destination, retValue);
                        //destinationProperty.SetValue(destination, LoadEntityFromDto<typeSource, typeDestination>(nestedObject));
                    }
                    //If property from 'EMD' namespace
                    else if (sourceProperty.PropertyType.Namespace.Contains("SDTF") && !typeof(IList).IsAssignableFrom(sourceProperty.PropertyType.GetGenericTypeDefinition()) && !typeof(System.Collections.Generic.List<>).IsAssignableFrom(sourceProperty.PropertyType.GetGenericTypeDefinition()) && !typeof(ICollection).IsAssignableFrom(sourceProperty.PropertyType.GetGenericTypeDefinition()) && !typeof(System.Collections.Generic.ICollection<>).IsAssignableFrom(sourceProperty.PropertyType.GetGenericTypeDefinition()))
                    {
                        var nestedObject = sourceProperty.GetValue(Source);
                        Type typeSource = nestedObject.GetType();
                        Type typeDestination = destinationProperty.PropertyType;
                        MethodInfo method = typeof(AutoMapper).GetMethod("LoadEntityFromDto")
                             .MakeGenericMethod(new Type[] { typeSource, typeDestination });
                        var retValue = method.Invoke(null, new object[] { nestedObject });
                        destinationProperty.SetValue(destination, retValue);
                        //destinationProperty.SetValue(destination, LoadEntityFromDto<typeSource, typeDestination>(nestedObject));
                    }
                    ///If property is type of generic list call method to convert DTO list to Entity list
                    else if (typeof(IList).IsAssignableFrom(sourceProperty.PropertyType.GetGenericTypeDefinition()) || typeof(System.Collections.Generic.List<>).IsAssignableFrom(sourceProperty.PropertyType.GetGenericTypeDefinition()))
                    {
                        var nestedObject = sourceProperty.GetValue(Source);
                        Type typeSource = sourceProperty.PropertyType.GenericTypeArguments[0];  //nestedObject.GetType();
                        Type typeDestination = destinationProperty.PropertyType.GenericTypeArguments[0];
                        MethodInfo method = typeof(AutoMapper).GetMethod("LoadEnitityListFromDtoList")
                             .MakeGenericMethod(new Type[] { typeSource, typeDestination });
                        var retValue = method.Invoke(null, new object[] { nestedObject });
                        destinationProperty.SetValue(destination, retValue);
                    }
                    ///If property is type of generic queryable call method to convert DTO list to Entity list
                    else if (typeof(ICollection).IsAssignableFrom(sourceProperty.PropertyType.GetGenericTypeDefinition()) || typeof(System.Collections.Generic.ICollection<>).IsAssignableFrom(sourceProperty.PropertyType.GetGenericTypeDefinition()))
                    {
                        var nestedObject = sourceProperty.GetValue(Source);
                        Type typeSource = sourceProperty.PropertyType.GenericTypeArguments[0];  //nestedObject.GetType();
                        Type typeDestination = destinationProperty.PropertyType.GenericTypeArguments[0];

                        MethodInfo method = typeof(AutoMapper).GetMethod("LoadEnitityListFromDtoList")
                             .MakeGenericMethod(new Type[] { typeSource, typeDestination });
                        var retValue = method.Invoke(null, new object[] { nestedObject });
                        destinationProperty.SetValue(destination, retValue);
                    }
                    ///If property is type of generic queryable call method to convert DTO list to Entity list
                    else if (typeof(IEnumerable).IsAssignableFrom(sourceProperty.PropertyType.GetGenericTypeDefinition()) || typeof(System.Collections.Generic.IEnumerable<>).IsAssignableFrom(sourceProperty.PropertyType.GetGenericTypeDefinition()))
                    {
                        var nestedObject = sourceProperty.GetValue(Source);
                        Type typeSource = sourceProperty.PropertyType.GenericTypeArguments[0];  //nestedObject.GetType();
                        Type typeDestination = destinationProperty.PropertyType.GenericTypeArguments[0];

                        MethodInfo method = typeof(AutoMapper).GetMethod("LoadEnitityListFromDtoIenumerable")
                             .MakeGenericMethod(new Type[] { typeSource, typeDestination });
                        var retValue = method.Invoke(null, new object[] { nestedObject });
                        destinationProperty.SetValue(destination, retValue);
                    }
                }
            }

            //Call Custom AutoMapper Method to flatten hierarchy
            //if (Source != null && destination != null)
            //{
            //    CustomAutoMapper.AutoMapping<T, U>(ref Source, destination);
            //}

            //CustomAutoMapper.IgnoreAllNonExisting(Mapper.CreateMap<T, U>());
            //Mapper.AssertConfigurationIsValid();
            //destination = Mapper.Map<T, U>(Source, destination);
            //destination = CustomAutoMapper.AutoMapping<T, U>(ref Source, destination);

            return destination;
        }

        /// <summary>
        /// Converts DTO list to Entity list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static List<U> LoadEnitityListFromDtoList<T, U>(List<T> Source)
        //where T : BaseDTO
        //where U : BaseEntity
        {
            //U[] source = Source.ToArray();
            //CustomAutoMapper.IgnoreAllNonExisting(Mapper.CreateMap<T, U>());
            ////  Mapper.CreateMap<T, U>();
            //List<T> destination = Mapper.Map<U[], List<T>>(source);
            //destination = CustomAutoMapper.AutoMapping<T, U>(ref Source, destination);
            //return destination;
            List<U> destination = null;
            if (Source != null && Source.Count() > 0)
            {
                destination = new List<U>();
                U destinationObject;
                foreach (T item in Source)
                {
                    if (item != null)
                    {
                        destinationObject = LoadEntityFromDto<T, U>(item);
                        if (destinationObject != null)
                            destination.Add(destinationObject);
                    }
                }
            }
            return destination;
        }

        public static List<U> LoadEnitityListFromDtoIenumerable<T, U>(IEnumerable<T> Source)
        //where T : BaseDTO
        //where U : BaseEntity
        {
            //U[] source = Source.ToArray();
            //CustomAutoMapper.IgnoreAllNonExisting(Mapper.CreateMap<T, U>());
            ////  Mapper.CreateMap<T, U>();
            //List<T> destination = Mapper.Map<U[], List<T>>(source);
            //destination = CustomAutoMapper.AutoMapping<T, U>(ref Source, destination);
            //return destination;
            List<U> destination = null;
            if (Source != null && Source.Count() > 0)
            {
                destination = new List<U>();
                U destinationObject;
                foreach (T item in Source)
                {
                    destinationObject = LoadEntityFromDto<T, U>(item);
                    destination.Add(destinationObject);
                }
            }
            return destination;

        }

        /// <summary>
        /// Generic Mapper for Domain Entity to DTO
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="Source"></param>
        /// <param name="destination"></param>
        public static U LoadDtoFromEntity<T, U>(T Source)
        //where T : BaseEntity
        //where U : BaseDTO
        {
            //U Destination = Activator.CreateInstance<U>();
            ////Mapper.CreateMap<T, U>();
            //CustomAutoMapper.IgnoreAllNonExisting(Mapper.CreateMap<T, U>());
            //Mapper.AssertConfigurationIsValid();
            //Destination = Mapper.Map<T, U>(Source, Destination);
            //Destination = AutoMappingDTO<T, U>(ref Source, Destination);
            //return Destination;
            return LoadDtoFromEntity<T, U>(Source, null);
        }

        /// <summary>
        /// Overloaded method to convert DTO from Entity List with provided ignore map.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="Source"></param>
        /// <param name="IgnoreMap"></param>
        /// <returns></returns>
        public static U LoadDtoFromEntity<T, U>(T Source, List<Type> IgnoreMap)
        //where T : BaseEntity
        //where U : BaseDTO
        {
            U destination = Activator.CreateInstance<U>();
            foreach (PropertyInfo sourceProperty in Source.GetType().GetProperties())
            {
                PropertyInfo destinationProperty = destination.GetType().GetProperty(sourceProperty.Name);
                if (destinationProperty != null)
                {
                    //No need to cater Refernce DTO. They are already handled in CustomAutomapper call
                    //MappingAttribute mappingAttribute = Utility.GetAutoMappingAttribute(sourceProperty);
                    //if (mappingAttribute != null)
                    //    continue;

                    //For primitive types assign values directly
                    if (sourceProperty.PropertyType.IsValueType || sourceProperty.PropertyType == typeof(System.String))
                    {
                        destinationProperty.SetValue(destination, sourceProperty.GetValue(Source));
                    }

                    //If property is type of composite object, call method recursively using reflection
                    else if (sourceProperty.PropertyType.IsSubclassOf(typeof(T)))
                    {
                        //If object is in ignore map dont assign to DTO
                        if ((IgnoreMap == null || !IgnoreMap.Contains(sourceProperty.PropertyType.UnderlyingSystemType)))
                        {
                            if (sourceProperty.GetValue(Source) != null)
                            {
                                var nestedObject = sourceProperty.GetValue(Source);
                                Type typeSource = nestedObject.GetType();
                                Type typeDestination = destinationProperty.PropertyType;
                                //MethodInfo method = typeof(AutoMapper).GetMethod("LoadDtoFromEntity", new Type[] { typeSource, typeof(List<Type>) })
                                //     .MakeGenericMethod(new Type[] { typeSource, typeDestination });

                                var allMethods = typeof(AutoMapper).GetMethods(BindingFlags.Public | BindingFlags.Static);
                                MethodInfo method = allMethods.FirstOrDefault(
                                    mi => mi.Name == "LoadDtoFromEntity" && mi.GetParameters().Count() == 2).MakeGenericMethod(new Type[] { typeSource, typeDestination });

                                var retValue = method.Invoke(null, new object[] { nestedObject, IgnoreMap });
                                destinationProperty.SetValue(destination, retValue);
                                //destinationProperty.SetValue(destination, LoadEntityFromDto<typeSource, typeDestination>(nestedObject));
                            }
                        }
                    }
                    ///If property is type of generic list call method to convert DTO list to Entity list
                    else if (typeof(System.Collections.Generic.IList<>).IsAssignableFrom(sourceProperty.PropertyType.GetGenericTypeDefinition()) || typeof(System.Collections.Generic.List<>).IsAssignableFrom(sourceProperty.PropertyType.GetGenericTypeDefinition()))
                    {
                        if (IgnoreMap == null || !IgnoreMap.Contains(sourceProperty.PropertyType.GenericTypeArguments[0]))
                        {
                            if (sourceProperty.GetValue(Source) != null)
                            {
                                var nestedObject = sourceProperty.GetValue(Source);
                                Type typeSource = sourceProperty.PropertyType.GenericTypeArguments[0];  //nestedObject.GetType();
                                Type typeDestination = destinationProperty.PropertyType.GenericTypeArguments[0];
                                //MethodInfo method = typeof(AutoMapper).GetMethod("LoadDtoListFromEntityList", new Type[] { typeSource, typeof(List<Type>) })
                                //     .MakeGenericMethod(new Type[] { typeSource, typeDestination });
                                var allMethods = typeof(AutoMapper).GetMethods(BindingFlags.Public | BindingFlags.Static);
                                MethodInfo method = allMethods.FirstOrDefault(
                                    mi => mi.Name == "LoadDtoListFromEntityList" && mi.GetParameters().Count() == 2);
                                method = method.MakeGenericMethod(new Type[] { typeSource, typeDestination });
                                //if (IgnoreMap != null)
                                //{
                                var retValue = method.Invoke(null, new object[] { nestedObject, IgnoreMap });
                                destinationProperty.SetValue(destination, retValue);
                                //}
                            }
                        }
                    }
                }
            }

            //Call Custom AutoMapper Method to flatten hierarchy
            //if (Source != null && destination != null)
            //{
            //    CustomAutoMapper.AutoMappingDTO<T, U>(ref Source, destination);
            //}
            return destination;
        }



        public static U ObjectToObjectMapper<T, U>(T Source, List<Type> IgnoreMap)
        {
            U destination = Activator.CreateInstance<U>();
            foreach (PropertyInfo sourceProperty in Source.GetType().GetProperties())
            {
                PropertyInfo destinationProperty = destination.GetType().GetProperty(sourceProperty.Name);
                if (destinationProperty != null)
                {
                    //For primitive types assign values directly
                    if (sourceProperty.PropertyType.IsValueType || sourceProperty.PropertyType == typeof(System.String))
                    {
                        destinationProperty.SetValue(destination, sourceProperty.GetValue(Source));
                    }
                }
            }
            return destination;
        }


        public static List<U> ListObjectToListObjectMapper<T, U>(List<T> Source)
        {
            List<U> destination = null;
            if (Source != null && Source.Count() > 0)
            {
                destination = new List<U>();
                U destinationObject;
                foreach (T item in Source)
                {
                    destinationObject = ObjectToObjectMapper<T, U>(item, null);
                    destination.Add(destinationObject);
                }
            }
            return destination;
        }


        /// <summary>
        /// Converts DTO list to Entity list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static List<U> LoadDtoListFromEntityList<T, U>(List<T> Source, List<Type> IgnoreMap)
        //where T : BaseEntity
        //where U : BaseDTO
        {
            //T[] source = Source.ToArray();
            //CustomAutoMapper.IgnoreAllNonExisting(Mapper.CreateMap<T, U>());
            ////Mapper.CreateMap<T, U>();
            //List<U> destination = Mapper.Map<T[], List<U>>(source);
            //destination = AutoMappingDTO<T, U>(ref source, destination);
            //return destination;
            List<U> destination = null;
            if (Source != null && Source.Count() > 0)
            {
                destination = new List<U>();
                U destinationObject;
                foreach (T item in Source)
                {
                    destinationObject = LoadDtoFromEntity<T, U>(item, IgnoreMap);
                    destination.Add(destinationObject);
                }
            }
            return destination;
        }

        /// <summary>
        /// Overloaded method to convert Entity list to DTO list with Ignore Map
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static List<U> LoadDtoListFromEntityList<T, U>(List<T> Source)
        //where T : BaseEntity
        //where U : BaseDTO
        {
            //T[] source = Source.ToArray();
            //CustomAutoMapper.IgnoreAllNonExisting(Mapper.CreateMap<T, U>());
            ////Mapper.CreateMap<T, U>();
            //List<U> destination = Mapper.Map<T[], List<U>>(source);
            //destination = AutoMappingDTO<T, U>(ref source, destination);
            //return destination;
            return LoadDtoListFromEntityList<T, U>(Source, null);
            //List<U> destination = new List<U>();
            //U destinationObject;
            //foreach (T item in Source)
            //{
            //    destinationObject = LoadDtoFromEntity<T, U>(item);
            //    destination.Add(destinationObject);
            //}
            //return destination;
        }


        ///// <summary>
        ///// 
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <typeparam name="U"></typeparam>
        ///// <param name="Source"></param>
        ///// <param name="destination"></param>
        ///// <returns></returns>
        //[Obsolete]
        //public static U AutoMapping<T, U>(ref T Source, U Destination)
        //{
        //    var typeDTO = typeof(T);
        //    List<PropertyInfo> propertiesDTO = typeDTO.GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(MappingAttribute))).ToList();
        //    foreach (PropertyInfo property in propertiesDTO)
        //    {
        //        MappingAttribute mappingAttribute = Utility.GetAutoMappingAttribute(property);
        //        if (mappingAttribute != null)
        //        {
        //            if (!mappingAttribute.IgnoreMapping)
        //            {
        //                BaseEntity baseEntity = (BaseEntity)Activator.CreateInstance("EHS.DomainEntities", mappingAttribute.MappingEntity).Unwrap();
        //                baseEntity.GetType().GetProperty(mappingAttribute.MappingProperty).SetValue(baseEntity, property.GetValue(Source));
        //                if (mappingAttribute.MappingPropertyValue != null)
        //                {
        //                    PropertyInfo basePropety = Destination.GetType().GetProperty(mappingAttribute.MappingPropertyValue);
        //                    if (basePropety != null)
        //                        basePropety.SetValue(Destination, baseEntity);
        //                }
        //                else
        //                {
        //                    PropertyInfo basePropety = Destination.GetType().GetProperty(property.Name);
        //                    if (basePropety != null)
        //                        basePropety.SetValue(Destination, baseEntity);
        //                }
        //                property.SetValue(Source, null);
        //            }
        //        }
        //    }
        //    return Destination;
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <typeparam name="U"></typeparam>
        ///// <param name="Source"></param>
        ///// <param name="destination"></param>
        ///// <returns></returns>
        //[Obsolete]
        //private static List<U> AutoMapping<T, U>(ref T[] Source, List<U> Destinations)
        //{
        //    List<U> lstDestination = new List<U>();
        //    foreach (var Destination in Destinations)
        //    {
        //        var typeDTO = typeof(T);
        //        List<PropertyInfo> propertiesDTO = typeDTO.GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(MappingAttribute))).ToList();
        //        foreach (PropertyInfo property in propertiesDTO)
        //        {
        //            MappingAttribute mappingAttribute = Utility.GetAutoMappingAttribute(property);
        //            if (mappingAttribute != null)
        //            {
        //                if (!mappingAttribute.IgnoreMapping)
        //                {
        //                    BaseEntity baseEntity = (BaseEntity)Activator.CreateInstance("EHS.DomainEntities", mappingAttribute.MappingEntity).Unwrap();
        //                    baseEntity.GetType().GetProperty(mappingAttribute.MappingProperty).SetValue(baseEntity, property.GetValue(Source));
        //                    if (mappingAttribute.MappingPropertyValue != null)
        //                    {
        //                        PropertyInfo basePropety = Destination.GetType().GetProperty(mappingAttribute.MappingPropertyValue);
        //                        if (basePropety != null)
        //                            basePropety.SetValue(Destination, baseEntity);
        //                    }
        //                    else
        //                    {
        //                        PropertyInfo basePropety = Destination.GetType().GetProperty(property.Name);
        //                        if (basePropety != null)
        //                            basePropety.SetValue(Destination, baseEntity);
        //                    }
        //                    property.SetValue(Source, null);
        //                }
        //            }
        //        }
        //        lstDestination.Add(Destination);
        //    }
        //    return lstDestination;
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <typeparam name="U"></typeparam>
        ///// <param name="Source"></param>
        ///// <param name="destination"></param>
        ///// <returns></returns>
        //[Obsolete]
        //public static U AutoMappingDTO<T, U>(ref T Source, U Destination)
        //{
        //    var typeDTO = typeof(U);
        //    List<PropertyInfo> propertiesDTO = typeDTO.GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(MappingAttribute))).ToList();
        //    foreach (PropertyInfo property in propertiesDTO)
        //    {
        //        MappingAttribute mappingAttribute = Utility.GetAutoMappingAttribute(property);
        //        if (mappingAttribute != null)
        //        {
        //            if (!mappingAttribute.IgnoreMapping)
        //            {
        //                if (mappingAttribute.MappingPropertyValue != null)
        //                {
        //                    PropertyInfo basePropety = Source.GetType().GetProperty(mappingAttribute.MappingPropertyValue);
        //                    if (basePropety != null)
        //                    {
        //                        BaseEntity baseEntity = (BaseEntity)basePropety.GetValue(Source);

        //                        if (baseEntity != null)
        //                            property.SetValue(Destination, baseEntity.GetType().GetProperty(mappingAttribute.MappingProperty).GetValue(baseEntity));
        //                    }
        //                }
        //                else
        //                {
        //                    PropertyInfo basePropety = Source.GetType().GetProperty(property.Name);
        //                    if (basePropety != null)
        //                    {
        //                        BaseEntity baseEntity = (BaseEntity)basePropety.GetValue(Source);

        //                        if (baseEntity != null)
        //                            property.SetValue(Destination, baseEntity.GetType().GetProperty(mappingAttribute.MappingProperty).GetValue(baseEntity));
        //                    }
        //                }
        //            }
        //            else if (mappingAttribute.MappingProperty != null)
        //            {
        //                PropertyInfo basePropety = Source.GetType().GetProperty(property.Name);
        //                if (basePropety != null)
        //                {
        //                    BaseEntity baseEntity = (BaseEntity)basePropety.GetValue(Source);

        //                    if (baseEntity != null)
        //                        property.SetValue(Destination, baseEntity.GetType().GetProperty(mappingAttribute.MappingProperty).GetValue(baseEntity));
        //                }
        //            }
        //            else
        //                property.SetValue(Destination, null);
        //        }
        //        else
        //            property.SetValue(Destination, null);
        //    }
        //    return Destination;
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <typeparam name="U"></typeparam>
        ///// <param name="Source"></param>
        ///// <param name="destination"></param>
        ///// <returns></returns>
        //[Obsolete]
        //public static List<U> AutoMappingDTO<T, U>(ref T[] Sources, List<U> Destinations)
        //{
        //    int index = 0;
        //    List<U> lstDestination = new List<U>();
        //    foreach (var Source in Sources)
        //    {
        //        U Destination = Destinations[index];
        //        var typeDTO = typeof(U);
        //        List<PropertyInfo> propertiesDTO = typeDTO.GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(MappingAttribute))).ToList();
        //        foreach (PropertyInfo property in propertiesDTO)
        //        {
        //            MappingAttribute mappingAttribute = Utility.GetAutoMappingAttribute(property);
        //            if (mappingAttribute != null)
        //            {
        //                if (!mappingAttribute.IgnoreMapping)
        //                {
        //                    if (mappingAttribute.MappingPropertyValue != null)
        //                    {
        //                        PropertyInfo basePropety = Source.GetType().GetProperty(mappingAttribute.MappingPropertyValue);
        //                        if (basePropety != null)
        //                        {
        //                            BaseEntity baseEntity = (BaseEntity)basePropety.GetValue(Source);
        //                            property.SetValue(Destination, baseEntity.GetType().GetProperty(mappingAttribute.MappingProperty).GetValue(baseEntity));
        //                        }
        //                    }
        //                    else
        //                    {
        //                        PropertyInfo basePropety = Source.GetType().GetProperty(property.Name);
        //                        if (basePropety != null)
        //                        {
        //                            BaseEntity baseEntity = (BaseEntity)basePropety.GetValue(Source);
        //                            property.SetValue(Destination, baseEntity.GetType().GetProperty(mappingAttribute.MappingProperty).GetValue(baseEntity));
        //                        }
        //                    }
        //                }
        //                else if (mappingAttribute.MappingProperty != null)
        //                {
        //                    PropertyInfo basePropety = Source.GetType().GetProperty(property.Name);
        //                    if (basePropety != null)
        //                    {
        //                        BaseEntity baseEntity = (BaseEntity)basePropety.GetValue(Source);
        //                        property.SetValue(Destination, baseEntity.GetType().GetProperty(mappingAttribute.MappingProperty).GetValue(baseEntity));
        //                    }
        //                }
        //                else
        //                    property.SetValue(Destination, null);
        //            }
        //            else
        //                property.SetValue(Destination, null);
        //        }
        //        lstDestination.Add(Destination);
        //        index++;
        //    }
        //    return lstDestination;
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <typeparam name="U"></typeparam>
        ///// <param name="Source"></param>
        ///// <param name="destination"></param>
        ///// <returns></returns>
        //[Obsolete]
        //public static List<T> AutoMapping<T, U>(ref List<U> Source, List<T> Destination)
        //{
        //    U[] source = Source.ToArray();
        //    int itrateDestination = 0;
        //    foreach (U dtoSource in source)
        //    {
        //        T destination = Destination[itrateDestination];
        //        List<PropertyInfo> propertiesDTO = dtoSource.GetType().GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(MappingAttribute))).ToList();
        //        foreach (PropertyInfo property in propertiesDTO)
        //        {
        //            MappingAttribute mappingAttribute = Utility.GetAutoMappingAttribute(property);
        //            if (mappingAttribute != null)
        //            {
        //                BaseEntity baseEntity = (BaseEntity)Activator.CreateInstance("EHS.DomainEntities", mappingAttribute.MappingEntity).Unwrap();
        //                baseEntity.GetType().GetProperty(mappingAttribute.MappingProperty).SetValue(baseEntity, property.GetValue(Source));
        //                PropertyInfo basePropety = destination.GetType().GetProperty(property.Name);
        //                if (basePropety != null)
        //                    basePropety.SetValue(destination, baseEntity);
        //                property.SetValue(Source, null);
        //            }
        //        }
        //    }

        //    return Destination;
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="destination"></param>
        ///// <returns></returns>
        //[Obsolete]
        //private static List<object> AutoMapping(List<object> Destination)
        //{
        //    List<object> lstEntity = new List<object>();
        //    foreach (var dto in Destination)
        //    {
        //        if (!dto.GetType().IsGenericType)
        //        {
        //            var proptylst = dto.GetType().GetCustomAttributes(typeof(MappingAttribute));
        //            foreach (MappingAttribute property in proptylst)
        //            {
        //                if (property.MappingEntity != null)
        //                {
        //                    var item = Activator.CreateInstance("EHS.DomainEntities", property.MappingEntity);
        //                    object baseEntity = (object)item.Unwrap();
        //                    lstEntity.Add(baseEntity);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            foreach (var lstDTO in dto.GetType().GetGenericArguments())
        //            {
        //                var proptylst = lstDTO.GetType().GetCustomAttributes(typeof(MappingAttribute));
        //                foreach (MappingAttribute property in proptylst)
        //                {
        //                    if (property.MappingEntity != null)
        //                    {
        //                        var item = Activator.CreateInstance("EHS.DomainEntities", property.MappingEntity);
        //                        object baseEntity = (object)item.Unwrap();
        //                        lstEntity.Add(baseEntity);
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return lstEntity;
        //}

        /////// <summary>
        /////// Generic Mapper for Domain Entity to DTO
        /////// </summary>
        /////// <typeparam name="T"></typeparam>
        /////// <typeparam name="U"></typeparam>
        /////// <param name="Source"></param>
        /////// <param name="destination"></param>
        ////public static U LoadDtoFromEntity<T, U>(T Source, U Destination)
        ////    where T : BaseEntity
        ////    where U : BaseDTO
        ////{
        ////    Mapper.CreateMap<T, U>();
        ////    Mapper.AssertConfigurationIsValid();
        ////    Destination = Mapper.Map<T, U>(Source, Destination);
        ////    return Destination;
        ////}

        ////public static U LoadMarineDtoFromEntity<T, U>(T Source)
        ////    where T : EHS.DomainEntities.Entities.MarineNOCMaster
        ////    where U : EHS.DTO.Marine.MarineMasterDTO
        ////{
        ////    U destination = Activator.CreateInstance<U>();
        ////    Mapper.CreateMap<EHS.DTO.Marine.MarineMasterDTO, EHS.DomainEntities.Entities.MarineNOCMaster>().ForMember(dest => dest.BunkeringOperation,option => option.Ignore());

        ////    destination = Mapper.Map<T, U>(Source, destination);
        ////    return destination;
        ////}


    }
}