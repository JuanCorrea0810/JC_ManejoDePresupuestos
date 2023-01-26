
using ElasticEmailClient;



namespace ManejoDePresupuestos.Servicios
{
    public interface IServicioEmail
    {
        Task ConfirmarCuenta(string correo, string urlRetorno);
        Task RecuperarContraseña(string correo, string urlRetorno);
    }

    public class ServicioEmailElasticEmail : IServicioEmail
    {
        private readonly IConfiguration configuration;

        public ServicioEmailElasticEmail(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public async Task ConfirmarCuenta(string correo, string urlRetorno)
        {
            string subject = "Confirmar Cuenta - JC_ManejoDePresupuestos ";
            string fromEmail = "juancorrear08102@gmail.com";
            var Destinatario = new List<string>() { correo };
            string bodyText = "Gracias por utilizar nuestros servicios";
            string bodyHtml = $"Por favor confirme su cuenta dando click aquí: <a href='{urlRetorno}'>enlace</a>";
            

            ApiTypes.EmailSend result = null;
            Api.ApiKey= configuration.GetValue<string>("ApiKey");
            

            try
            {
                result = await Api.Email.SendAsync(subject: subject,from: fromEmail,bodyText: bodyText, bodyHtml: bodyHtml, to: Destinatario);
               
            }
            catch (Exception ex)
            {
                if (ex is ApplicationException)
                    Console.WriteLine("Server didn't accept the request: " + ex.Message);
                else
                    Console.WriteLine("Something unexpected happened: " + ex.Message);

                return;
            }

            Console.WriteLine("MsgID to store locally: " + result.MessageID); // Available only if sent to a single recipient
            Console.WriteLine("TransactionID to store locally: " + result.TransactionID);
        }

        public async Task RecuperarContraseña(string correo, string urlRetorno)
        {
            var subject = "Recuperar Contraseña - JC_ManejoDePresupuestos";
            string fromEmail = "juancorrear08102@gmail.com";
            var Destinatario = new List<string>() { correo };
            string bodyText = "Gracias por utilizar nuestros servicios";
            string bodyHtml = $"Recupere su contraseña click aquí: <a href='{urlRetorno}'>enlace</a>";
            ApiTypes.EmailSend result = null;
            Api.ApiKey = configuration.GetValue<string>("ApiKey");
            try
            {
                result = await Api.Email.SendAsync(subject: subject, from: fromEmail, bodyText: bodyText, bodyHtml: bodyHtml, to: Destinatario);

            }
            catch (Exception ex)
            {
                if (ex is ApplicationException)
                    Console.WriteLine("Server didn't accept the request: " + ex.Message);
                else
                    Console.WriteLine("Something unexpected happened: " + ex.Message);

                return;
            }

            Console.WriteLine("MsgID to store locally: " + result.MessageID); // Available only if sent to a single recipient
            Console.WriteLine("TransactionID to store locally: " + result.TransactionID);
        }
    }
}


