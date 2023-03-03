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
            CreateMap<CuentaViewModel, Cuenta>().ReverseMap();
            CreateMap<Cuenta, MostrarCuentaViewModel>().ForMember(x=> x.TipoCuenta , z=> z.MapFrom(o=> o.TipoCuentas.Nombre)).ReverseMap();
            CreateMap<CuentaViewModel, CuentaCreacionViewModel>();
            CreateMap<CategoríaViewModel,Categoria>().ReverseMap();
            CreateMap<TransaccionViewModel, Transaccion>().ReverseMap();
            CreateMap<Transaccion, TransaccionCreacionViewModel>().ForMember(x=> x.TipoOperacionId , z=> z.MapFrom(o=> o.Categoria.TipoOperacionId))
                                                                  .ForMember(x=> x.Cuenta,z=> z.MapFrom(o=> o.Cuenta.Nombre))
                                                                  .ForMember(x=> x.Categoria,z=> z.MapFrom(o=> o.Categoria.Nombre)).ReverseMap();
            
            CreateMap<TransaccionCreacionViewModel,ActualizarTransaccionViewModel>().ForMember(x=> x.CuentaAnteriorId,z=>z.MapFrom(o=> o.CuentaId))
                                                                                    .ForMember(x=> x.MontoAnterior, z=>z.MapFrom(o=> o.Monto));
            CreateMap<ActualizarTransaccionViewModel, Transaccion>();
       

        }
    }
}
