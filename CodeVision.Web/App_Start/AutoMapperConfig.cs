using System.Collections.Generic;
using AutoMapper;
using CodeVision.Model;

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

            Mapper.CreateMap<CodeVision.CSharp.Semantic.Module, ViewModels.Module>();
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
    }    
}