using eVidaGeneralLib.DAO.Util;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO.SCL;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.DAO.SCL {
	internal class SclItemListaDAO {


		internal static string CheckItemLista(string cdLista, string id, EvidaDatabase db) {
			if (string.IsNullOrEmpty(id))
				return id;
			string descricao = id.ToUpper();
			id = descricao;
			id = FormatUtil.EnsureCapacity(id, 6);

			DataTable dtItemLista = LocatorDAO.ListarItensLista(cdLista, db);
			int ordem = 1;
			if (dtItemLista != null) {
				IEnumerable<DataRow> lst = from item in dtItemLista.AsEnumerable()
										   select item;

				if (lst != null && lst.Count() > 0) {
					DataRow drFound = lst.FirstOrDefault(dr => dr.Field<string>("CD_ITEM_LISTA").Equals(id));
					if (drFound != null) {
						return drFound.Field<string>("CD_ITEM_LISTA");
					} else {
						ordem = lst.Max(x => Convert.ToInt32(x["CD_ORDEM_EXIBICAO"])) + 1;
					}
				}
			}
			SclItemListaVO vo = new SclItemListaVO();
			vo.CdSistema = "HC3";
			vo.CdLista = cdLista;
			vo.Id = id;
			vo.OrdemExibicao = ordem;
			vo.Descricao = descricao;
			vo.Ativo = true;
			vo.Unico = false;

			CriarItemLista(vo, db);
			return vo.Id;
		}

		private static void CriarItemLista(SclItemListaVO vo, EvidaDatabase evdb) {

			string sql = "INSERT INTO ISA_SCL.SCL_ITEM_LISTA (CD_SISTEMA, CD_LISTA, CD_ITEM_LISTA, DS_ITEM_LISTA, CD_ORDEM_EXIBICAO, " +
				"	FG_UNICO, FG_ATIVO, USER_CREATE, DATE_CREATE) " +
				" VALUES " +
				" (:sistema, :lista, :id, :nome, :ordem, " +
				"	:fgUnico, :fgAtivo, :userInt, :dateInt) ";

			Database db = evdb.Database;
			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":sistema", DbType.String, vo.CdSistema);
			db.AddInParameter(dbCommand, ":lista", DbType.String, vo.CdLista);
			db.AddInParameter(dbCommand, ":id", DbType.String, vo.Id);

			db.AddInParameter(dbCommand, ":nome", DbType.String, vo.Descricao);
			db.AddInParameter(dbCommand, ":ordem", DbType.Int32, vo.OrdemExibicao);

			db.AddInParameter(dbCommand, ":fgUnico", DbType.String, vo.Unico ? "S" : "N");
			db.AddInParameter(dbCommand, ":fgAtivo", DbType.String, vo.Ativo ? "S" : "N");

			db.AddInParameter(dbCommand, ":userInt", DbType.String, "INTRANET - INTEGRAÇÃO");
			db.AddInParameter(dbCommand, ":dateInt", DbType.String, DateTime.Now.ToString("dd/MM/yyyy HH:mm"));

			BaseDAO.ExecuteNonQuery(dbCommand, evdb);
		}
	}
}
