using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.Util {
	public class FileUtil {
		private static EVidaLog log = new EVidaLog(typeof(FileUtil));

		public enum FileDir {
			DECLARACAO_UNIVERSITARIO,
			AUTORIZACAO,
			ASSINATURA,
			RECIPROCIDADE,
			REUNIAO,
			CONSELHO,
			INDISPONIBILIDADE_REDE,
			SOLICITACAO_VIAGEM,
			PROTOCOLO_CANAL_GESTANTE,
			DECLARACAO_ANUAL_DEBITO,
			MENSALIDADE_IR,
            MEDICAMENTO_REEMBOLSAVEL,
            BOLETIM_OCORRENCIA
		}
		//The extension of the specified path (including the period ".")
		public static string GetFileExtension(string fileName) {
			return Path.GetExtension(fileName);
		}

        public static string GetFilePath(FileDir categoria, string subFolder, string id, string fileName)
        {
            string catDir = GetRepositoryDir(categoria);
            if (!string.IsNullOrEmpty(subFolder))
                catDir = GetRepositoryDir(categoria, subFolder);

            string newFile = fileName;
            if (!string.IsNullOrEmpty(id) && !newFile.StartsWith(id + "_"))
                newFile = id + "_" + newFile;

            newFile = Path.Combine(catDir, newFile).ToLower();
            newFile = Path.GetFullPath(newFile);
            return newFile;
        }
		
		public static string GetFileFromPathWithoutExtension(FileDir categoria, string id, string fileNameWithoutExtension) {
			log.Debug("GetFilePathWithoutExtension(" + categoria + ", " + id + ", " + fileNameWithoutExtension + ")");
			string catDir = GetRepositoryDir(categoria);
			string newFile = fileNameWithoutExtension;
			if (!string.IsNullOrEmpty(id) && !newFile.StartsWith(id))
				newFile = id + "_" + newFile;

			catDir = Path.GetFullPath(catDir);

			log.Debug("EnumerateFiles(" + catDir + ", " + newFile + ")");
			IEnumerable<string> files = Directory.EnumerateFiles(catDir, newFile + "*", SearchOption.TopDirectoryOnly);
			log.Debug("EnumerateFiles(" + catDir + ", " + newFile + ") => " + files.Count());
			string realFileName = null;
			foreach (string fileName in files) {
				//log.Debug(fileName);
				//log.Debug(Path.GetFileNameWithoutExtension(fileName));
				if (newFile.Equals(Path.GetFileNameWithoutExtension(fileName), StringComparison.InvariantCultureIgnoreCase)) {
					log.Debug(fileName + " - " + Path.GetFileNameWithoutExtension(fileName));
					realFileName = fileName;
					break;
				}
			}

			return realFileName;
		}

		public static string GetRepositoryDir(FileDir categoria, bool fullPath = false) {
			string serverDir = ParametroUtil.FileRepository;
			serverDir = Path.Combine(serverDir, categoria.ToString());

			if (!Directory.Exists(serverDir))
				Directory.CreateDirectory(serverDir);

			if (fullPath)
				serverDir = Path.GetFullPath(serverDir);
			return serverDir;
		}

		public static string GetRepositoryDir(FileDir categoria, string subFolder, bool fullPath = false) {
			string serverDir = GetRepositoryDir(categoria, fullPath);
			serverDir = Path.Combine(serverDir, subFolder);

			if (!Directory.Exists(serverDir))
				Directory.CreateDirectory(serverDir);
			return serverDir;
		}

		public static string MoverArquivo(string dirDestino, string fileName) {
			return MoverArquivo(null, null, fileName, dirDestino, null); 
		}

		public static string MoverArquivo(string id, string dirOrigem, string fileOrigem, string dirDestino, string fileDestino) {
			log.Debug("Moving file: " + id + " from: " + dirOrigem + " - " + fileOrigem + " to: " + dirDestino + " - " + fileDestino);
			if (string.IsNullOrEmpty(fileOrigem))
				return null;

			if (dirOrigem == null)
				dirOrigem = Path.GetTempPath();

			string old = Path.Combine(dirOrigem, fileOrigem);
			
			string newFile = fileDestino;
			if (newFile == null)
				newFile = fileOrigem;
			if (!string.IsNullOrEmpty(id) && !newFile.StartsWith(id + "_"))
				newFile = id + "_" + newFile;

			if (!Directory.Exists(dirDestino))
				Directory.CreateDirectory(dirDestino);

			string newFinalPath = Path.Combine(dirDestino, newFile).ToLower();

			File.Copy(old, newFinalPath, true);
			return newFile;
		}

		internal static bool HasTempFile(string diskFile, string dirOrigem = null) {
			if (dirOrigem == null)
				dirOrigem = Path.GetTempPath();
			string old = Path.Combine(dirOrigem, diskFile);
			return (File.Exists(old));
		}

		internal static void RemoverArquivo(string id, string dirDestino, string nomeArq) {
			string newFile = nomeArq;
			if (!string.IsNullOrEmpty(id) && !newFile.StartsWith(id))
				newFile = id + "_" + newFile;
			
			newFile = Path.Combine(dirDestino, newFile).ToLower();
			if (File.Exists(newFile)) {
				File.Delete(newFile);
			}
		}

		public static string SaveFile(string dir, string fileName, byte[] bytes) {
			string newFile = Path.Combine(dir, fileName).ToLower();
			if (File.Exists(newFile)) {
				File.Delete(newFile);
			}
			File.WriteAllBytes(newFile, bytes);
			return newFile;
		}
	}
}
