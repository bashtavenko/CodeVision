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

            Mapper.CreateMap<ViewModels.SearchHit, Model.Hit>()
                .ConstructUsing(x => new Hit(x.FilePath, x.ContentRootPath, x.Score, x.BestFragment, x.Offsets));
        }
    }    
}