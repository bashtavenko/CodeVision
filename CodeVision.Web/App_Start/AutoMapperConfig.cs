using System;
using System.Collections.Generic;
using AutoMapper;
using CodeVision.Dependencies.Database;
using CodeVision.Dependencies.SqlStorage;
using CodeVision.Model;
using DatabaseObject = CodeVision.Dependencies.Database.DatabaseObject;
using DatabaseObjectProperty = CodeVision.Web.ViewModels.DatabaseObjectProperty;
using Module = CodeVision.Dependencies.Modules.Module;
using System.Linq;

namespace CodeVision.Web
{
    public class AutoMapperConfig
    {
        public static void CreateMaps()
        {
            // from, to
            Mapper.CreateMap<Model.Hit, ViewModels.SearchHit>()
                .ForMember(s => s.PrismCssClassName, opt => opt.ResolveUsing<PrismCssClassLanguageResolver>());

            Mapper.CreateMap<Model.ReadOnlyHitCollection, ViewModels.SearchResult>()
                .ForMember(s => s.Hits, opt => opt.MapFrom(src => src));

            Mapper.CreateMap<Model.Offset, ViewModels.SearchOffset>();
            Mapper.CreateMap<ViewModels.SearchOffset, Model.Offset>();

            Mapper.CreateMap<ViewModels.SearchHit, Model.Hit>()
                .ConstructUsing(x => new Hit(x.DocId, x.FilePath, x.ContentRootPath, x.Score, x.BestFragment, MapOffsets(x.Offsets), x.Language));

            Mapper.CreateMap<ViewModels.SearchHit, ViewModels.Document>()
                .ForMember(s => s.Name, opt => opt.MapFrom(src => src.FriendlyFileName));

            Mapper.CreateMap<Module, ViewModels.Module>();

            Mapper.CreateMap<DatabaseObject, ViewModels.DatabaseObject>();
            Mapper.CreateMap<ViewModels.DatabaseObject, DatabaseObject>();

            Mapper.CreateMap<ObjectProperty, ViewModels.DatabaseObjectProperty>().ConvertUsing<ObjectPropertyTypeConverter>();
            Mapper.CreateMap<ViewModels.DatabaseObjectProperty, ObjectProperty>().ConvertUsing<DatabaseObjectPropertyTypeConverter>();
            Mapper.CreateMap<DatabaseObjectPropertyType, ViewModels.DatabaseObjectProperty>().ConvertUsing<DatabaseObjectPropertyTypeConverter2>();

            Mapper.CreateMap<Dependencies.SqlStorage.Package, ViewModels.Package>()
                .ForMember(s => s.Id, opt => opt.MapFrom(src => src.PackageId));

            Mapper.CreateMap<Dependencies.SqlStorage.Project, ViewModels.Project>();

            Mapper.CreateMap<Dependencies.DependencyMatrix, ViewModels.DependencyMatrix>()
                .ForMember(s => s.Packages, opt => opt.MapFrom(src => src.Rows))
                .ForMember(s => s.Projects, opt => opt.MapFrom(src => src.Columns));
        }
        
        private static List<Model.Offset> MapOffsets(List<ViewModels.SearchOffset> offsets)
        {
            return Mapper.Map<List<Model.Offset>>(offsets);
        }

        private class PrismCssClassLanguageResolver : ValueResolver<Model.Hit, string>
        {
            protected override string ResolveCore(Model.Hit source)
            {
                switch (source.Language)
                {
                    case "cs":
                        return "language-csharp";
                    case "js":
                        return "language-javascript";
                    case "sql":
                        return "language-sql";
                    default:
                        return "language-markup";
                }
            }
        }

        private class ObjectPropertyTypeConverter :  ITypeConverter<ObjectProperty, ViewModels.DatabaseObjectProperty>
        {
            public ViewModels.DatabaseObjectProperty Convert(ResolutionContext context)
            {
                var source = context.SourceValue as ObjectProperty;
                var result = new ViewModels.DatabaseObjectProperty();

                if (source is RelevantToFinancialReportingProperty)
                {
                    result.PropertyType = DatabaseObjectPropertyType.RelevantToFinancialReporting;
                }

                if (source is CommentProperty)
                {
                    result.PropertyType = DatabaseObjectPropertyType.Comment;
                    result.PropertyValue = ((CommentProperty) source).Text;
                }
                result.PropertyName = ConvertDatabaseObjectPropertyToString(result.PropertyType);

                return result;
            }
        }

        private class DatabaseObjectPropertyTypeConverter : ITypeConverter<ViewModels.DatabaseObjectProperty, ObjectProperty>
        {
            public ObjectProperty Convert(ResolutionContext context)
            {
                var source = context.SourceValue as ViewModels.DatabaseObjectProperty;
                switch (source.PropertyType)
                {
                    case DatabaseObjectPropertyType.RelevantToFinancialReporting:
                        return new RelevantToFinancialReportingProperty();
                    case DatabaseObjectPropertyType.Comment:
                        return new CommentProperty(source.PropertyValue);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private class DatabaseObjectPropertyTypeConverter2 : ITypeConverter<DatabaseObjectPropertyType, ViewModels.DatabaseObjectProperty>
        {
            public DatabaseObjectProperty Convert(ResolutionContext context)
            {
                var source = (DatabaseObjectPropertyType) context.SourceValue;
                return new DatabaseObjectProperty
                {
                    PropertyType = source,
                    PropertyName = ConvertDatabaseObjectPropertyToString(source),
                };
            }
        }

        private static string ConvertDatabaseObjectPropertyToString(DatabaseObjectPropertyType source)
        {
            switch (source)
            {
                case DatabaseObjectPropertyType.RelevantToFinancialReporting:
                    return "Relevant to Financial Reporting";
                case DatabaseObjectPropertyType.Comment:
                    return "Comment";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }    
}