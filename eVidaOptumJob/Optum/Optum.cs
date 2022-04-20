using System;
using System.Data;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Configuration;
using System.Threading.Tasks;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.Filter;
using eVidaGeneralLib.ReadWriteCsv;

namespace eVidaOptumJob.Optum
{
    class Optum
    {
        private EVidaLog log = new EVidaLog(typeof(Optum));
        private long idProcesso;

        private string DIA_ATUAL;
        private string MES_ATUAL;
        private string ANO_ATUAL;

        private DateTime INICIO_ANO_ANTERIOR;
        private DateTime INICIO_SEMESTRE_ANTERIOR;

        private string ARQUIVO_ZIP;

        #region[CONSTANTES]

        private const string CAMINHO_PLANILHA = @"c:\eVida_mig\export_files\OPTUM\PLANILHAS\";
        private const string CAMINHO_ZIP = @"c:\eVida_mig\export_files\OPTUM\ZIP\";

        private const string ARQUIVO_GERAIS = @"SolGeraisInterV4.csv";
        private const string ARQUIVO_ATIVOS = @"CadastroAtivos.csv";
        private const string ARQUIVO_EXCLUSOES = @"CadastroExclusoes.csv";
        private const string ARQUIVO_MOVIMENTACOES = @"CadastroMovimentacoes.csv";
        private const string ARQUIVO_INTERNACOES = @"Internacoes.csv";
        private const string ARQUIVO_MEDICAMENTOS = @"Medicamentos.csv";

        private const string SENHA_ARQUIVO = @"Optum";
        private const string CAMINHO_NAS = @"\\evidasrv14\Departamental\Gerência de Atenção a Saúde\8 - Gestão de crônicos\16 Base_Dados_Optum\ARQUIVOS\";
        private const string USUARIO_FTP = @"optum";
        private const string SENHA_FTP = @"0ptum@2018";
        private const string ENDERECO_FTP = @"ftp.e-vida.org.br";
        private const string DIRETORIO_FTP = @"/home/optum/out";
        private const int PORTA = 22;

        #endregion

        public void Run()
        {
            log.Info("Run");
            idProcesso = 0;

            DIA_ATUAL = DateTime.Today.Day.ToString().PadLeft(2, '0');
            MES_ATUAL = DateTime.Today.Month.ToString().PadLeft(2, '0');
            ANO_ATUAL = DateTime.Today.Year.ToString();

            INICIO_ANO_ANTERIOR = new DateTime(Int32.Parse(ANO_ATUAL) - 1, 01, 01);

            if (Int32.Parse(MES_ATUAL) <= 6) INICIO_SEMESTRE_ANTERIOR = new DateTime(Int32.Parse(ANO_ATUAL) - 1, 07, 01);
            else INICIO_SEMESTRE_ANTERIOR = new DateTime(Int32.Parse(ANO_ATUAL), 01, 01);

            ARQUIVO_ZIP = "OPTUM_" + DIA_ATUAL + MES_ATUAL + ANO_ATUAL + ".zip";

            try
            {
                idProcesso = ProcessoBO.Instance.RegistrarProcesso(ControleProcessoEnum.GERACAO_ENVIO_ARQUIVO_OPTUM);
                log.Debug("Processo: " + idProcesso);

                GerarArquivos();
                CompactarArquivos();
                //ConfigurarDiretorioEvida();
                CopiarArquivoEvida();
                //ConfigurarDiretorioFTP();
                TransferirArquivoFTP();
                EnvioEmailEnvolvidos();

                int qtd = 1;
                ProcessoBO.Instance.SucessoProcesso(idProcesso, ControleProcessoEnum.GERACAO_ENVIO_ARQUIVO_OPTUM, qtd, null);
            }
            catch (Exception ex)
            {
                log.Error("Erro ao processar", ex);
                if (idProcesso != 0)
                    ProcessoBO.Instance.ErroProcesso(idProcesso, ControleProcessoEnum.GERACAO_ENVIO_ARQUIVO_OPTUM, ex);
            }
            log.Info("End");
        }

        #region[GERAÇÃO DOS ARQUIVOS]

        private void GerarArquivos()
        {
            GerarArquivoSolGeraisInterV4();
            GerarArquivoCadastroAtivos();
            GerarArquivoCadastroExclusoes();
            GerarArquivoCadastroMovimentacoes();
            GerarArquivoInternacoes();
            //GerarArquivoMedicamentos();
        }

        private void GerarArquivoSolGeraisInterV4()
        {
            try
            {
                System.Data.DataTable dt = OptumBO.Instance.BuscarSolGeraisInterV4(INICIO_SEMESTRE_ANTERIOR);
                GerarArquivoExcel(dt, CAMINHO_PLANILHA, ARQUIVO_GERAIS);
                log.Debug("Gerado o arquivo " + ARQUIVO_GERAIS + ".");
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar o arquivo " + ARQUIVO_GERAIS + ". " + ex.Message);
            }
        }

        private void GerarArquivoCadastroAtivos()
        {
            try
            {
                System.Data.DataTable dt = OptumBO.Instance.BuscarCadastroAtivos();
                GerarArquivoExcel(dt, CAMINHO_PLANILHA, ARQUIVO_ATIVOS);
                log.Debug("Gerado o arquivo " + ARQUIVO_ATIVOS + ".");
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar o arquivo " + ARQUIVO_ATIVOS + ". " + ex.Message);
            }
        }

        private void GerarArquivoCadastroExclusoes()
        {
            try
            {
                System.Data.DataTable dt = OptumBO.Instance.BuscarCadastroExclusoes(INICIO_ANO_ANTERIOR);
                GerarArquivoExcel(dt, CAMINHO_PLANILHA, ARQUIVO_EXCLUSOES);
                log.Debug("Gerado o arquivo " + ARQUIVO_EXCLUSOES + ".");
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar o arquivo " + ARQUIVO_EXCLUSOES + ". " + ex.Message);
            }
        }

        private void GerarArquivoCadastroMovimentacoes()
        {
            try
            {
                System.Data.DataTable dt = OptumBO.Instance.BuscarCadastroMovimentacoes(INICIO_ANO_ANTERIOR);
                GerarArquivoExcel(dt, CAMINHO_PLANILHA, ARQUIVO_MOVIMENTACOES);
                log.Debug("Gerado o arquivo " + ARQUIVO_MOVIMENTACOES + ".");
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar o arquivo " + ARQUIVO_MOVIMENTACOES + ". " + ex.Message);
            }
        }

        private void GerarArquivoInternacoes()
        {
            try
            {
                System.Data.DataTable dt = OptumBO.Instance.BuscarInternacoes(INICIO_SEMESTRE_ANTERIOR);
                GerarArquivoExcel(dt, CAMINHO_PLANILHA, ARQUIVO_INTERNACOES);
                log.Debug("Gerado o arquivo " + ARQUIVO_INTERNACOES + ".");
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar o arquivo " + ARQUIVO_INTERNACOES + ". " + ex.Message);
            }
        }

        private void GerarArquivoMedicamentos()
        {
            try
            {
                System.Data.DataTable dt = OptumBO.Instance.BuscarMedicamentos();
                GerarArquivoExcel(dt, CAMINHO_PLANILHA, ARQUIVO_MEDICAMENTOS);
                log.Debug("Gerado o arquivo " + ARQUIVO_MEDICAMENTOS + ".");
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar o arquivo " + ARQUIVO_MEDICAMENTOS + ". " + ex.Message);
            }
        }

        private void GerarArquivoExcel(System.Data.DataTable dt, string caminho, string arquivo)
        {
            // Caso já exista, exclui o arquivo
            File.Delete(caminho + arquivo);

            // Write data to CSV file
            using (CsvFileWriter writer = new CsvFileWriter(caminho + arquivo))
            {
                // Escreve as linhas de títulos
                CsvRow row = new CsvRow();
                foreach (DataColumn col in dt.Columns)
                {
                    row.Add(col.ColumnName);
                }
                writer.WriteRow(row);

                // Escreve as linhas de dados
                foreach (DataRow dr in dt.Rows)
                {
                    row = new CsvRow();

                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        string texto = dr[i].ToString();

                        // Tratamento no texto
                        texto = texto.Replace("\n", "");
                        texto = texto.Replace("¿", "");
                        texto = texto + "\t";

                        row.Add(texto);
                    }

                    writer.WriteRow(row);
                }
            }
        }

        #endregion

        #region[COMPACTAÇÃO DOS ARQUIVOS]

        private void CompactarArquivos()
        {

            // Caso já exista, exclui o arquivo
            File.Delete(CAMINHO_ZIP + ARQUIVO_ZIP);

            try
            {
                ZipUtil.CreateSample(CAMINHO_ZIP + ARQUIVO_ZIP, SENHA_ARQUIVO, CAMINHO_PLANILHA);
                log.Debug("Gerado o arquivo compactado.");
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar o arquivo compactado. " + ex.Message);
            }

        }

        #endregion

        #region[CONFIGURAÇÃO DO DIRETÓRIO E-VIDA]

        private void ConfigurarDiretorioEvida()
        {

            string diretorio_evida_ano = CAMINHO_NAS + ANO_ATUAL + @"\";
            string diretorio_evida_mes = CAMINHO_NAS + ANO_ATUAL + @"\" + MES_ATUAL + @"\";

            try
            {

                bool existe_diretorio_evida_ano = Directory.Exists(diretorio_evida_ano);
                bool existe_diretorio_evida_mes = Directory.Exists(diretorio_evida_mes);

                if (!existe_diretorio_evida_ano)
                {
                    Directory.CreateDirectory(diretorio_evida_ano);
                }

                if (!existe_diretorio_evida_ano)
                {
                    Directory.CreateDirectory(diretorio_evida_mes);
                }

                log.Debug("Configurado o diretório na E-Vida.");

            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar configurar o diretório na E-Vida. " + ex.Message);
            }

        }

        #endregion

        #region[CÓPIA DO ARQUIVO PARA E-VIDA]

        private void CopiarArquivoEvida()
        {

            // Caso já exista, exclui o arquivo
            File.Delete(CAMINHO_NAS + ARQUIVO_ZIP);

            try
            {

                File.Copy(CAMINHO_ZIP + ARQUIVO_ZIP, CAMINHO_NAS + ARQUIVO_ZIP);

                log.Debug("Copiado o arquivo para a E-Vida.");

            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao copiar o arquivo para a E-Vida. " + ex.Message);
            }
        }

        #endregion

        #region[CONFIGURAÇÃO DO DIRETÓRIO FTP]

        private void ConfigurarDiretorioFTP()
        {
            try
            {
                // Criando um diretório FTP
                string diretorio_ftp_ano = DIRETORIO_FTP + ANO_ATUAL;
                FTPUtil.CreateSFTPDirectory(ENDERECO_FTP, USUARIO_FTP, SENHA_FTP, diretorio_ftp_ano, PORTA);

                string diretorio_ftp_mes = DIRETORIO_FTP + ANO_ATUAL + @"/" + MES_ATUAL;
                FTPUtil.CreateSFTPDirectory(ENDERECO_FTP, USUARIO_FTP, SENHA_FTP, diretorio_ftp_mes, PORTA);

                log.Debug("Configurado o diretório no FTP.");

            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao configurar o diretório no FTP. " + ex.Message);
            }
        }

        #endregion

        #region[TRANSFERÊNCIA DO ARQUIVO PARA O FTP]

        private void TransferirArquivoFTP()
        {

            // Enviando um arquivo por FTP
            string txtArquivo = CAMINHO_NAS + ARQUIVO_ZIP;
            string txtDiretorioFTP = DIRETORIO_FTP;

            // Caso já exista, exclui o arquivo
            //FTPUtil.DeletarArquivoFTP(ENDERECO_FTP, USUARIO_FTP, SENHA_FTP, txtDiretorioFTP, ARQUIVO_ZIP);

            try
            {
                // Envia o arquivo
                FTPUtil.UploadSFTPFile(ENDERECO_FTP, USUARIO_FTP, SENHA_FTP, txtArquivo, txtDiretorioFTP, PORTA);

                log.Debug("Transferido o arquivo para o FTP.");
            }
            catch (WebException ex)
            {
                throw new Exception("Erro ao transferir o arquivo para o FTP. " + ((FtpWebResponse)ex.Response).StatusDescription);
            }
        }

        #endregion

        #region[ENVIO DE E-MAIL PARA OS ENVOLVIDOS]

        private void EnvioEmailEnvolvidos()
        {

            try
            {

                OptumBO.Instance.EnviarEmailEnvolvidos();

                log.Debug("Enviados os e-mails de alerta.");

            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao enviar os e-mails de alerta. " + ex.Message);
            }


        }

        #endregion

    }
}
