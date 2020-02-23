using System;
using System.Text.Json;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;
using Dapper.Contrib.Extensions;
using ServerlessMonitorSitesSQL.Models;
using ServerlessMonitorSitesSQL.Clients;

namespace ServerlessMonitorSitesSQL
{
    public class ChecagemTimerTrigger
    {
        private readonly TesteSiteClient _testeSiteClient;

        public ChecagemTimerTrigger(TesteSiteClient testeSiteClient)
        {
            _testeSiteClient = testeSiteClient;
        }

        [FunctionName("ChecagemTimerTrigger")]
        public void Run([TimerTrigger("*/30 * * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation(
                $"ChecagemTimerTrigger - iniciando execução em: {DateTime.Now}");

            var sites = Environment.GetEnvironmentVariable("Sites")
                .Split("|", StringSplitOptions.RemoveEmptyEntries);
            foreach (string site in sites)
            {
                using (var conexao = new SqlConnection(
                    Environment.GetEnvironmentVariable("BaseMonitoramentoSites")))
                {
                    var dadosLog = new LogMonitoramento()
                    {
                        Site = site,
                        Horario = DateTime.Now,
                    };

                    try
                    {
                        // Envio da requisicao a fim de determinar se
                        // o site esta no ar
                        var response = _testeSiteClient.EnviarRequisicaoTeste(site);

                        dadosLog.Status = (int)response.StatusCode + " " +
                            response.StatusCode;
                        if (response.StatusCode != System.Net.HttpStatusCode.OK)
                            dadosLog.DescricaoErro = response.ReasonPhrase;
                    }
                    catch (Exception ex)
                    {
                        dadosLog.Status = "Exception";
                        dadosLog.DescricaoErro = ex.Message;
                    }

                    string jsonResultado =
                        JsonSerializer.Serialize(dadosLog);

                    if (dadosLog.DescricaoErro == null)
                        log.LogInformation(jsonResultado);
                    else
                        log.LogError(jsonResultado);

                    conexao.Insert(dadosLog);
                }
            }

            log.LogInformation(
                $"ChecagemTimerTrigger - concluindo execução em: {DateTime.Now}");
        }
    }
}