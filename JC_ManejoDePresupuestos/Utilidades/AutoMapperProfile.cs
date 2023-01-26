using AutoMapper;
using ManejoDePresupuestos.Models;
using ManejoDePresupuestos.Utilidades.Administracion_DTOs;

namespace ManejoDePresupuestos.Utilidades
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<TipoCuentaViewModel, TipoCuenta>();
            CreateMap<TipoCuenta,MostrarTipoCuentaViewModel>().ReverseMap();
            CreateMap<PutUsersDTO, NewIdentityUser>().ReverseMap();
            
        }
    }
}
