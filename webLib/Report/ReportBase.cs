using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using eVida.Web.Security;
using eVidaGeneralLib.VO.HC;
using SkyReport.ReportingServices;
using eVidaGeneralLib.Reporting;

namespace eVida.Web.Report {
	
	public abstract class ReportBase<T, U> : SkyReport.ReportingServices.ReportBase<T> where U:IReportBinder {

		private U Binder { get; set; }
		public IUsuarioLogado Usuario { get; private set; }
		protected eVidaGeneralLib.Util.EVidaLog Log { get; private set; }

		public ReportBase(string reportDir, IUsuarioLogado usuario) : base(reportDir) {
			Usuario = usuario;
			Log = new eVidaGeneralLib.Util.EVidaLog(typeof(T));
		}

		protected U GetBinder(T vo) {
			if (Binder == null) {
				Binder = CreateBinder(vo);
			}
			return Binder;
		}

		protected abstract U CreateBinder(T vo);

		protected override string GerarNome(T vo) {
			IReportBinder binder = GetBinder(vo);
			return binder.GerarNome();
		}

		protected override string DefaultRpt(T vo) {
			IReportBinder binder = GetBinder(vo);
			return binder.DefaultRpt();
		}

		protected override T GerarDados(HttpContext context) {
			return GerarDados(context.Request, context.Session);
		}

		internal virtual T GerarDados(HttpRequest request, HttpSessionState session) {
			return GerarDados(request);
		}

		internal abstract T GerarDados(HttpRequest request);

        protected bool CheckUsuarioBeneficiario(string codint, string codemp, string matric)
        {
			UsuarioBeneficiarioVO uVO = Usuario as UsuarioBeneficiarioVO;
			//Se a requisição foi pelo beneficiários, então verificar a matrícula
			if (uVO != null) {
                if (uVO.Codint.Trim() == codint.Trim() && uVO.Codemp.Trim() == codemp.Trim() && uVO.Matric.Trim() == matric.Trim())
					return true;
				else
					return false;
			}
			return true;
		}

		protected override void FillReport(T vo, ReportParams parameters, RelatorioHelper helper) {
			U binder = GetBinder(vo);
			ReportBinderParams repParams = binder.GetData();
			if (repParams.UseExternalImages) {
				helper.EnableExternalImages = true;
			}
			FillFromBinder(parameters, repParams);
		}

		protected void FillFromBinder(ReportParams repParam, ReportBinderParams repBinder) {
			foreach (string key in repBinder.DataSources.Keys) {
				repParam.DataSources.Add(key, repBinder.DataSources[key]);
			}

			foreach (string key in repBinder.Params.Keys) {
				repParam.Params.Add(key, repBinder.Params[key]);
			}
		}
	}
}