using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Renci.SshNet;

namespace eVidaGeneralLib.Util
{
    public class FTPUtil
    {
        static EVidaLog log = new EVidaLog(typeof(FTPUtil));

        public static bool CriarDiretorioFTP(string directoryPath, string usuario, string senha)
        {
            bool existe = true;
            try
            {
                WebRequest request = WebRequest.Create(directoryPath);
                request.Method = WebRequestMethods.Ftp.MakeDirectory;
                request.Credentials = new NetworkCredential(usuario, senha);

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            }
            catch
            {
                existe = false;
            }
            return existe;
        }

        public static void DeletarDiretorioFTP(string url, string usuario, string senha)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url);
                request.Method = WebRequestMethods.Ftp.RemoveDirectory;
                request.Credentials = new NetworkCredential(usuario, senha);
                request.GetResponse().Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void EnviarArquivoFTP(string arquivo, string url, string usuario, string senha)
        {
            try
            {
                FileInfo arquivoInfo = new FileInfo(arquivo);
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(url));
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(usuario, senha);
                request.UseBinary = true;
                request.ContentLength = arquivoInfo.Length;
                using (FileStream fs = arquivoInfo.OpenRead())
                {
                    byte[] buffer = new byte[2048];
                    int bytesSent = 0;
                    int bytes = 0;
                    using (Stream stream = request.GetRequestStream())
                    {
                        while (bytesSent < arquivoInfo.Length)
                        {
                            bytes = fs.Read(buffer, 0, buffer.Length);
                            stream.Write(buffer, 0, bytes);
                            bytesSent += bytes;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void BaixarArquivoFTP(string url, string local, string usuario, string senha)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(url));
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Credentials = new NetworkCredential(usuario, senha);
                request.UseBinary = true;
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    using (Stream rs = response.GetResponseStream())
                    {
                        using (FileStream ws = new FileStream(local, FileMode.Create))
                        {
                            byte[] buffer = new byte[2048];
                            int bytesRead = rs.Read(buffer, 0, buffer.Length);
                            while (bytesRead > 0)
                            {
                                ws.Write(buffer, 0, bytesRead);
                                bytesRead = rs.Read(buffer, 0, buffer.Length);
                            }
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public static void MoverArquivoFTP(string ftpURL, string UserName, string Password, string ftpDirectory, string ftpDirectoryProcessed, string FileName)
        {
            FtpWebRequest ftpRequest = null;
            FtpWebResponse ftpResponse = null;
            try
            {
                ftpRequest = (FtpWebRequest)WebRequest.Create(ftpURL + "/" + ftpDirectory + "/" + FileName);
                ftpRequest.Credentials = new NetworkCredential(UserName, Password);
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                ftpRequest.Method = WebRequestMethods.Ftp.Rename;
                ftpRequest.RenameTo = ftpDirectoryProcessed + "/" + FileName;
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                ftpResponse.Close();
                ftpRequest = null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void DeletarArquivoFTP(string ftpURL, string UserName, string Password, string ftpDirectory, string FileName)
        {
            try
            {
                FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(ftpURL + "/" + ftpDirectory + "/" + FileName);
                ftpRequest.Credentials = new NetworkCredential(UserName, Password);
                ftpRequest.Method = WebRequestMethods.Ftp.DeleteFile;
                FtpWebResponse responseFileDelete = (FtpWebResponse)ftpRequest.GetResponse();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void UploadSFTPFile(string host, string username,
          string password, string sourcefile, string destinationpath, int port)
        {
            using (SftpClient client = new SftpClient(host, port, username, password))
            {
                client.Connect();
                client.ChangeDirectory(destinationpath);
                using (FileStream fs = new FileStream(sourcefile, FileMode.Open))
                {
                    client.BufferSize = 4 * 1024;
                    client.UploadFile(fs, Path.GetFileName(sourcefile));
                }
            }
        }

        public static void CreateSFTPDirectory(string host, string username,
          string password, string directorypath, int port)
        {
            using (SftpClient client = new SftpClient(host, port, username, password))
            {
                client.Connect();
                client.CreateDirectory(directorypath);
            }
        }
    }
}
