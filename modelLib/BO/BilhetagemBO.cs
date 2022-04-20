using eVidaGeneralLib.DAO;
using eVidaGeneralLib.Exceptions;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.BO {
	public class BilhetagemBO {
		EVidaLog log = new EVidaLog(typeof(BilhetagemBO));
		private static BilhetagemBO instance = new BilhetagemBO();

		public static BilhetagemBO Instance { get { return instance; } }

		public void ProcessarArquivo(string fullFilePath, out int rowNumber, out int ok, out int bad) {
			List<BilhetagemVO> lstBilhetes = ReadFile(fullFilePath, out ok, out bad, out rowNumber);
			if (lstBilhetes == null)
				return;
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();
				DbTransaction transaction = connection.BeginTransaction();
				try {
					log.Debug("Limpando temporária");
					BilhetagemDAO.ClearTemporarios(transaction, db);
					transaction.Commit();

					log.Debug("Inciando gravação em banco " + lstBilhetes.Count);				
					transaction = connection.BeginTransaction();
					BilhetagemDAO.IncluirTemporarios(lstBilhetes, transaction, db);
					transaction.Commit();


					log.Debug("Inciando processamento de diferença ");	
					transaction = connection.BeginTransaction();
					ok = BilhetagemDAO.ProcessarDiferenca(transaction, db);
					transaction.Commit();
					log.Debug("Diferenças: " + ok);
				} catch {
					transaction.Rollback();
					throw;
				} finally {
					connection.Close();
				}
			}
		}

		private List<BilhetagemVO> ReadFile(string fullFilePath, out int ok, out int bad, out int rowNumber) {
			string filePartNoExt = Path.GetFileNameWithoutExtension(fullFilePath);
			string badDir = ParametroUtil.GetParameter(ParametroUtil.ParametroType.BILHETAGEM_DIR_BAD);
			if (string.IsNullOrEmpty(badDir))
				badDir = ParametroUtil.GetParameter(ParametroUtil.ParametroType.BILHETAGEM_DIR_IN);
			string badFileName = Path.Combine(badDir, DateTime.Now.ToString("yyyyMMddHHmm") + "_" + filePartNoExt + ".bad");
			ok = 0;
			bad = 0;
			rowNumber = 0;

			List<BilhetagemVO> lstBilhetes = new List<BilhetagemVO>();
			using (StreamReader sr = new StreamReader(fullFilePath)) {
				string line;

				try {
					if (log.IsDebugEnabled)
						log.Debug("Inciando leitura do arquivo " + fullFilePath);

					DateTime start = DateTime.Now;
					string filePart = Path.GetFileName(fullFilePath);

					while ((line = sr.ReadLine()) != null) {
						rowNumber++;
						if (line.StartsWith("Data")) continue;

						try {
							BilhetagemVO vo = ParseLinha(filePart, rowNumber, line);
							lstBilhetes.Add(vo);
						} catch (ParseFileException pfex) {
							log.Error("Falha na linha " + rowNumber, pfex);
							WriteBadFile(badFileName, line);
							bad++;
						}
						if (rowNumber % 5000 == 0) {
							double milis = DateTime.Now.Subtract(start).TotalMilliseconds;
							log.Info("PARSING: " + rowNumber + " - " + milis);
							Console.WriteLine("PARSING: " + rowNumber + " - " + milis);
							start = DateTime.Now;
						}
					}

					if (log.IsDebugEnabled) {
						log.Debug("Rows: " + rowNumber + " - " + ok + " - " + bad);
						log.Debug("Arquivo lido: " + fullFilePath);
					}
				} catch (Exception ex) {
					log.Error("Erro ao ler arquivo", ex);
					Console.WriteLine("PROCESSO FINALIZADO COM ERRO");
					return null;
				}
			}
			return lstBilhetes;
		}
		
		/*
		public void ProcessarArquivo2(string fullFilePath, out int rowNumber, out int ok, out int bad) {
			string filePartNoExt = Path.GetFileNameWithoutExtension(fullFilePath);
			string badDir = ParametroUtil.GetParameter(ParametroUtil.ParametroType.BILHETAGEM_DIR_BAD);
			if (string.IsNullOrEmpty(badDir))
				badDir = ParametroUtil.GetParameter(ParametroUtil.ParametroType.BILHETAGEM_DIR_IN);
			string badFileName = Path.Combine(badDir, filePartNoExt + "_" + DateTime.Now.ToString("yyyyMMddHHmm") + ".bad");
			ok = 0;
			bad = 0;

			using (StreamReader sr = new StreamReader(fullFilePath)) {
				string line;

				Database db = DatabaseFactory.CreateDatabase();
				using (DbConnection connection = db.CreateConnection()) {
					connection.Open();
					DbTransaction transaction = connection.BeginTransaction();
					try {
						if (log.IsDebugEnabled)
							log.Debug("Inciando leitura do arquivo " + fullFilePath);

						BilhetagemVO lastVO = BilhetagemDAO.GetLast(transaction, db);
						string lastLine = null;
						if (lastVO != null)
							lastLine = GenerateFileLine(lastVO);

						rowNumber = 0;
						int rowProcessed = 0;
						DateTime start = DateTime.Now;
						string filePart = Path.GetFileName(fullFilePath);

						bool rowFound = string.IsNullOrEmpty(lastLine);

						if (!rowFound) {
							log.Info("SKIPPING");
						}

						while ((line = sr.ReadLine()) != null) {
							rowNumber++;
							if (line.StartsWith("Data")) continue;

							if (!rowFound) {
								if (line.Equals(lastLine)) {
									rowFound = true;
									log.Info("STOP SKIPPING");
								}
								continue;
							}

							rowProcessed++;
							try {
								ProcessarLinha(filePart, rowNumber, line, transaction, db);
								ok++;
							} catch (ParseFileException pfex) {
								log.Error("Falha na linha " + rowNumber, pfex);
								WriteBadFile(badFileName, line);
								bad++;
							}
							if (rowNumber % 1000 == 0) {
								double milis = DateTime.Now.Subtract(start).TotalMilliseconds;
								
								log.Info(rowNumber + " - " + milis);
								Console.WriteLine(rowNumber + " - " + milis);
								start = DateTime.Now;
								transaction.Commit();
								transaction = connection.BeginTransaction();
							}
						}

						if (log.IsDebugEnabled) {
							log.Debug("Rows: " + rowNumber + " - " + ok + " - " + bad);
							log.Debug("Arquivo lido: " + fullFilePath);
						}

						transaction.Commit();
					} catch {
						transaction.Rollback();
						throw;
					} finally {
						connection.Close();
					}
				}

			}
		}*/

		private void WriteBadFile(string filePath, string line) {
			using (StreamWriter sw = new StreamWriter(filePath, true)) {
				sw.WriteLine(line);
			}
		}

		private BilhetagemVO ParseLinha(string arquivo, int rowNumber, string linha) {
			BilhetagemVO vo = new BilhetagemVO();
			string[] linhaArr = linha.Split(new char[] { ';' });

			if (linhaArr.Length != 10) {
				throw new ParseFileException(rowNumber, -1, "A linha não possui 10 colunas!");
			}

			long lng;
			int number;
			DateTime date;
			string str;

			int pos = 0;
			if (!Int64.TryParse(linhaArr[pos].Trim(), out lng)) {
				throw new ParseFileException(rowNumber, pos, "Número de bilhete inválido!");
			}
			vo.NumeroBilhete = lng;

			pos++;
			if (!DateTime.TryParse(linhaArr[pos].Trim(), out date)) {
				throw new ParseFileException(rowNumber, pos, "Data inválida!");
			}
			vo.DataBilhetagem = date;

			//Data/Hora	Direção	Duração	Origem	Destino	Juntor	Conta	Estado	?
			//0 319668;
			//1 05/11/2015 18:25:25;
			//2 R;
			//3 112s;
			//4 6133669000;
			//5 68;
			//6 SIP_TRUNK_GVT;
			//7 ;
			//8 ATENDIDA;
			//9 GVT
			pos++;
			vo.Direcao = linhaArr[pos].Trim(); // Direção

			pos++;
			str = linhaArr[pos].Trim(); // Duração
			vo.DuracaoRaw = str;
			str = str.Remove(str.Length - 1); // remove o s do final
			if (!Int32.TryParse(str, out number)) {
				throw new ParseFileException(rowNumber, pos, "Duração/Número inválido!");
			}
			vo.Duracao = number;

			pos++;
			str = linhaArr[pos].Trim(); // Origem
			vo.OrigemRaw = str;
			int idx = str.LastIndexOf('#');
			if (idx >= 0) {
				str = str.Substring(idx + 1);
			}
			//if (!Int64.TryParse(str, out msisdn)) {
			//	throw new ParseFileException(rowNumber, 3, "Origem parse/Número inválido (" + str + ")!");
			//}
			vo.Origem = str;

			pos++;
			str = linhaArr[pos].Trim(); // Destino
			vo.DestinoRaw = str;
			idx = str.LastIndexOf('#');
			if (idx >= 0) {
				str = str.Substring(idx + 1);
			}
			//if (!Int64.TryParse(str, out msisdn)) {
			//	throw new ParseFileException(rowNumber, 4, "Destino parse/Número inválido (" + str + ")!");
			//}
			vo.Destino = str;

			pos++;
			vo.Juntor = linhaArr[pos].Trim(); // juntor
			pos++;
			vo.Conta = linhaArr[pos].Trim(); // conta
			pos++;
			vo.Estado = linhaArr[pos].Trim(); // estado
			pos++;
			vo.Conexao = linhaArr[pos].Trim(); // conexao
			
			vo.Arquivo = arquivo;
			vo.Linha = linha;
			return vo;
		}
		/*
		private void ProcessarLinha(string arquivo, int rowNumber, string linha, DbTransaction transaction, Database db) {
			BilhetagemVO vo = ParseLinha(arquivo, rowNumber, linha);
			IncluirLinha(vo, transaction, db);
		}
		
		private void IncluirLinha(BilhetagemVO vo, DbTransaction transaction, Database db) {
			BilhetagemDAO.IncluirBilhetagem(vo, transaction, db);
		}
		
		private string GenerateFileLine(BilhetagemVO vo) {
			//Data/Hora	Direção	Duração	Origem	Destino	Juntor	Conta	Estado	?
			return string.Format("{0:dd/MM/yyyy HH:mm:ss},{1},{2},{3},{4},{5},{6},{7},{8}",
				vo.DataBilhetagem, vo.Direcao, vo.DuracaoRaw, vo.OrigemRaw, vo.DestinoRaw, vo.Juntor, vo.Conta, vo.Estado, vo.Conexao);
		}
		*/
		private bool IsEquals(BilhetagemVO a, BilhetagemVO b) {
			if (a.DataBilhetagem != b.DataBilhetagem) return false;
			if (!string.Equals(a.Conexao,b.Conexao, StringComparison.InvariantCultureIgnoreCase)) return false;
			if (!string.Equals(a.Conta, b.Conta, StringComparison.InvariantCultureIgnoreCase)) return false;
			if (!string.Equals(a.DestinoRaw,b.DestinoRaw)) return false;
			if (!string.Equals(a.Direcao, b.Direcao)) return false;
			if (!string.Equals(a.DuracaoRaw, b.DuracaoRaw)) return false;
			if (!string.Equals(a.Estado, b.Estado)) return false;
			return true;
		}

		public DataTable Pesquisar(DateTime? dtInicial, DateTime? dtFinal, string direcao, List<int> lstSetores, List<int> lstRamais, List<string> lstEstados) {
			DataTable dt = null;
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();

				try {
					dt = BilhetagemDAO.Pesquisar(dtInicial, dtFinal, direcao, lstSetores, lstRamais, lstEstados, db);
				} finally {
					connection.Close();
				}
			}

			if (dt != null) {
				List<UsuarioVO> lstUsuarios = UsuarioBO.Instance.ListarUsuarios();
				List<RamalVO> lstRamalDb = RamalBO.Instance.ListarRamais();
				List<SetorUsuarioVO> lstSetorDb = SetorUsuarioBO.Instance.ListarSetores();

				foreach (DataRow dr in dt.Rows) {
					string tpDirecao = Convert.ToString(dr["TP_DIRECAO"]);
					dr["TP_DIRECAO"] = tpDirecao.Equals("R") ? "RECEBIDA" : tpDirecao.Equals("O") ? "ORIGINADA" : "I";

					string origem  = Convert.ToString(dr["RORIGEM_NR_RAMAL"]);
					string destino = Convert.ToString(dr["RDESTINO_NR_RAMAL"]);

					if (!string.IsNullOrEmpty(origem)) {
						int nrRamal = Convert.ToInt32(origem);
						origem = TraduzRamal(nrRamal, lstRamalDb, lstUsuarios, lstSetorDb);

					} else {
						origem = Convert.ToString(dr["DS_TEL_ORIGEM"]);
					}

					if (!string.IsNullOrEmpty(destino)) {
						int nrRamal = Convert.ToInt32(destino);
						destino = TraduzRamal(nrRamal, lstRamalDb, lstUsuarios, lstSetorDb);
					} else {
						destino = Convert.ToString(dr["DS_TEL_DESTINO"]);
					}

					dr["RORIGEM_DS_RAMAL"] = origem;
					dr["RDESTINO_DS_RAMAL"] = destino;
				}
			}
			return dt;
		}

		private string TraduzRamal(int nrRamal, List<RamalVO> lstRamal, List<UsuarioVO> lstUsuarios, List<SetorUsuarioVO> lstSetor) {
			RamalVO ramal = lstRamal.Find(x => x.NrRamal == nrRamal);
			string str = "";
			if (ramal.Tipo.Equals("SETOR")) {
				SetorUsuarioVO setorVO = lstSetor.Find(x => x.Id == ramal.IdSetor.Value);
				str = setorVO.Nome;
			} else {
				List<UsuarioVO> lstUsuRamal = lstUsuarios.FindAll(x => ramal.Usuarios.Contains(x.Id));
				str = lstUsuRamal.Select(x => GetFirstName(x.Nome)).Aggregate((x, y) => x + ", " + y);
			}
			return str + " - " + nrRamal;
		}

		private string GetFirstName(string name) {
			return name.Split(new char[] { ' ' })[0];
		}
	}


}
