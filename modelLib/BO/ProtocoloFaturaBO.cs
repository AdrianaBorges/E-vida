using eVidaGeneralLib.DAO;
using eVidaGeneralLib.DAO.HC;
using eVidaGeneralLib.DAO.Protheus;
using eVidaGeneralLib.DAO.Util;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.BO {
	public class ProtocoloFaturaBO {
		EVidaLog log = new EVidaLog(typeof(ProtocoloFaturaBO));

		private static ProtocoloFaturaBO instance = new ProtocoloFaturaBO();

		public static ProtocoloFaturaBO Instance { get { return instance; } }

		public DataTable Pesquisar(string nrProtocolo, string nrCpfCnpj, string razaoSocial, List<int> lstAnalistasResp, string docFiscal, DateTime? dtEmissao, decimal? vlApresentado,
            DateTime? dtEntradaInicio, DateTime? dtEntradaFim, /*StatusProtocoloFatura? status,*/ FaseProtocoloFatura? fase, DateTime? dtVencimentoInicio, DateTime? dtVencimentoFim,
            DateTime? dtFinalizacaoInicio, DateTime? dtFinalizacaoFim, DateTime? dtExpedicaoInicio, DateTime? dtExpedicaoFim, string cdNatureza, string cdRegional, int? controle)
        {
			
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return ProtocoloFaturaDAO.Pesquisar(nrProtocolo, nrCpfCnpj, razaoSocial, lstAnalistasResp, docFiscal, dtEmissao, vlApresentado, 
					dtEntradaInicio, dtEntradaFim, /*status,*/ fase, dtVencimentoInicio, dtVencimentoFim,dtFinalizacaoInicio, dtFinalizacaoFim, dtExpedicaoInicio, dtExpedicaoFim,
					cdNatureza, cdRegional, controle, db);				
			}
		}

		public ProtocoloFaturaVO GetById(int id) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				ProtocoloFaturaVO vo = ProtocoloFaturaDAO.GetById(id, db);
				if (vo != null) {
                    vo.RedeAtendimento = PRedeAtendimentoDAO.GetById(vo.RedeAtendimento.Codigo, db);
				}
				return vo;
			}
		}

        public bool ExisteProtocoloFatura(string codpeg)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return ProtocoloFaturaDAO.ExisteProtocoloFatura(codpeg, db);
            }
        }

		public void Gerar(ProtocoloFaturaVO vo, int idUsuario) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();

				vo.CdUsuarioCriacao = idUsuario;
				vo.AnoEntrada = DateTime.Now.Year;

                if (vo.DataVencimento == DateTime.MinValue) 
                {
                    PRedeAtendimentoVO redeVO = PRedeAtendimentoDAO.GetById(vo.RedeAtendimento.Codigo, db);
                    if (Int32.Parse(redeVO.Diapol.Trim()) != 0)
                    {
                        vo.DataVencimento = vo.DataEntrada.AddDays(Int32.Parse(redeVO.Diapol.Trim()));
                    }
                    else 
                    {
                        throw new Exception("A data de vencimento não está preenchida no Protheus e o credenciado não possui formula para cálculo!");
                    }
                }

			    ProtocoloFaturaDAO.Gerar(vo, db);

				connection.Commit();
			}
		}

		public void Salvar(ProtocoloFaturaVO vo) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();
				vo.AnoEntrada = vo.DataEntrada.Year;

				ProtocoloFaturaDAO.Salvar(vo, db);

				connection.Commit();
			}
		}

		public void Cancelar(int id, string motivo, int idUsuario) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();

				ProtocoloFaturaDAO.Cancelar(id, motivo, db);

				connection.Commit();
			}
		}

        public void Mesclar(string duplicados)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                connection.CreateTransaction();

                ProtocoloFaturaDAO.ExcluirCancelados(db);
                ProtocoloFaturaDAO.Mesclar(duplicados, db);

                connection.Commit();
            }
        }

		public void Assumir(int id, int idUsuario) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();

				ProtocoloFaturaDAO.Assumir(id, idUsuario, db);

				connection.Commit();
			}
		}

		public List<ProtocoloFaturaVO> FindAllActiveNotify() {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return ProtocoloFaturaDAO.FindAllActiveNotify(db);
			}
		}

		public void EnviarEmailAlerta(UsuarioVO usuario, List<ProtocoloFaturaVO> lstPendentes) {
			EmailUtil.ProtocoloFatura.SendAlerta(usuario, lstPendentes);
		}

        public DataTable ObterDuplicados()
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return ProtocoloFaturaDAO.ObterDuplicados(db);
            }
        }

	}
}
