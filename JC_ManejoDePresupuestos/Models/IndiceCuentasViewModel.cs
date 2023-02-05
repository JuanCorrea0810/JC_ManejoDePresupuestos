namespace ManejoDePresupuestos.Models
{
    public class IndiceCuentasViewModel
    {
        public string TipoCuenta { get; set; }
        public IEnumerable<MostrarCuentaViewModel> Cuentas { get; set; }
        public decimal Balance => Cuentas.Sum(x=> x.Balance);
    }
}
