using System.Collections.Generic;
using AutoMapper;
using CodeVision.Model;

namespace CodeVision.Web
{
    public class AutoMapperConfig
    {
        public static void CreateMaps()
        {
            Mapper.CreateMap<Model.Hit, ViewModels.SearchHit>();
            Mapper.CreateMap<Model.ReadOnlyHitCollection, ViewModels.SearchResult>()
                .ForMember(s => s.Hits, opt => opt.MapFrom(src => src));

            Mapper.CreateMap<Model.Offset, ViewModels.SearchOffset>();
            Mapper.CreateMap<ViewModels.SearchOffset, Model.Offset>();

            Mapper.CreateMap<ViewModels.SearchHit, Model.Hit>()
                .ConstructUsing(x => new Hit(x.DocId, x.FilePath, x.ContentRootPath, x.Score, x.BestFragment, MapOffsets(x.Offsets)));
        }

        private static List<Model.Offset> MapOffsets(List<ViewModels.SearchOffset> offsets)
        {
            return Mapper.Map<List<Model.Offset>>(offsets);
        }
    }    
}