<%@ Page Title="SOLICITAÇÃO DE AUTORIZAÇÃO" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="BuscaAutorizacao.aspx.cs" Inherits="eVidaCredenciados.Forms.BuscaAutorizacao" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        var POP_REVALIDAR = 1;
        function ConfigControlsCustom() {
            createLocator(650, 550, dlgOpen, dlgClose, defaultDlgCallback);
        }

        function openPop(tipo, obj, id, row) {
            var handler = new LocatorHandler(tipo, id, row, obj);
            var titulo = "";
            switch (tipo) {
                case POP_REVALIDAR: titulo = "Revalidar Solicitação"; break;
            }

            openLocator(titulo, handler);
            return false;
        }
        function dlgOpen(handler, ev, ui) {
            var src = "";
            switch (handler.tipo) {
                case POP_REVALIDAR: src = '../FormsPop/PopAutorizacaoRevalidar.aspx?'; break;
            }
            src += '&ID=' + handler.id;
            setLocatorUrl(src);
        }

        function dlgClose(handler, ev, ui) {
            switch (handler.tipo) {
                case POP_REVALIDAR: window.location.reload(); break;
            }
        }
        function openRevalidar(obj, id, row) {
            openPop(POP_REVALIDAR, obj, id, row);
            return false;
        }

    	function openEdit(id) {
    		window.location = './Autorizacao.aspx?ID=' + id;
    		return false;
    	}
    	function confirmExcluir() {
    		return confirm("Deseja realmente cancelar esta solicitação?");
    	}
    	function openReq() {
    		open('../docs/RELACAO_PROCEDIMENTO_AUTORIZACAO_PREVIA.pdf?DT=' + (new Date()).getDate(), 'docReq');
    	}

    	function openPdf(id, fName) {
    		openFile('<%= eVidaGeneralLib.Util.FileUtil.FileDir.AUTORIZACAO %>', "ID=" + id + ";" + fName);
    		return false;
    	}
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">

    <table style="text-align:center">
	<tr style="height:300px">
		<td colspan="4">
			<asp:Button ID="btnListaReq" runat="server" Text="Lista de Autorização Prévia" OnClientClick="return openReq()" />
            <asp:Button ID="btnNovo" runat="server" Text="Nova Solicitação" OnClick="btnNovo_Click" /><br /><br />
            <asp:Label ID="lblCount" runat="server" /><br /><br />
			<asp:GridView ID="gdvRelatorio" runat="server" AutoGenerateColumns="false" DataKeyNames="cd_autorizacao"
				AllowSorting="false" CssClass="tabela" Width="1400px" OnRowDataBound="gdvRelatorio_RowDataBound">
                <Columns>
					<asp:BoundField HeaderText="Protocolo" DataField="cd_autorizacao" DataFormatString="{0:000000000}" ItemStyle-HorizontalAlign="Right" />
					<asp:BoundField HeaderText="Protocolo ANS" DataField="NR_PROTOCOLO_ANS" />
					<asp:BoundField HeaderText="Beneficiário" DataField="benef_NM_NOMUSR" />
					<asp:BoundField HeaderText="Nº TISS" />
					<asp:BoundField HeaderText="Plano" DataField="plano_DESCRI" />
					<asp:BoundField HeaderText="Solicitante" DataField="TP_ORIGEM" />
					<asp:BoundField HeaderText="Caráter" DataField="TP_AUTORIZACAO" />
					<asp:BoundField HeaderText="Status" DataField="ST_AUTORIZACAO" />
                    <asp:BoundField HeaderText="Data Solicitação" DataField="dt_solicitacao" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" ItemStyle-HorizontalAlign="Center" />
					<asp:BoundField HeaderText="Data Aprovação" DataField="DT_AUTORIZACAO" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" ItemStyle-HorizontalAlign="Center" />                    
                    <asp:BoundField HeaderText="Data Sol. Revalidação" DataField="dt_sol_revalidacao" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" ItemStyle-HorizontalAlign="Center" />					
                    <asp:BoundField HeaderText="Data Aprov. Revalidação" DataField="dt_aprov_revalidacao" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" ItemStyle-HorizontalAlign="Center" />
                    <asp:TemplateField>
						<ItemStyle Width="80px" />
                        <ItemTemplate>
							<asp:ImageButton ID="btnEditar" runat="server" ImageUrl="~/img/ico_editar.gif" Height="20px" OnClientClick='<%# "return openEdit("+ Eval("cd_autorizacao") + ");" %>' />
							<asp:ImageButton ID="btnCancelar" runat="server" ImageUrl="~/img/remove.png" Height="20px" OnClientClick='<%# "return confirmExcluir();" %>' OnClick="btnCancelar_Click" CommandArgument='<%# Eval("cd_autorizacao") %>' />
                            <asp:ImageButton ID="btnRevalidar" runat="server" ImageUrl="~/img/refresh.png" Height="20px" OnClientClick='<%# CreateJsFunctionGrid(Container, "openRevalidar") %>'  />
                            <asp:ImageButton ID="btnPdf" runat="server" ImageUrl="~/img/PDF.png" Height="30px" Visible="false" />
                        </ItemTemplate>
                    </asp:TemplateField>
				</Columns>
			</asp:GridView>
		</td>
	</tr>

</table>
</asp:Content>
