<%@ Page Title="Consulta de Cobertura" Language="C#" MasterPageFile="~/Internal/ExternalPages.Master" AutoEventWireup="true" CodeBehind="ConsultaPlano.aspx.cs" Inherits="eVidaBeneficiarios.Externo.ConsultaPlano" %>
<%@ Register TagPrefix="evida" TagName="Header" Src="~/Externo/Header.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .tbInfoAdicional {
            border-collapse: collapse;
        }
        .tbInfoAdicional td {
            padding: 5px;
        }
        p {
            font-size: x-large;
        }
        select {
            font-size: large;
        }
        .p2 {
            font-size: large;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    <evida:Header id="header" runat="server" /><br />
    <div id="dvLogin" runat="server">
    <div class="lista">
		<div style="width: 450px">
		<fieldset>
			<legend>Para acessar informe os seguintes dados:</legend>
			<table width="100%">
                <tr>
                    <td colspan="2"><asp:Label ID="lblLoginExpirado" runat="server" Text="Sua sessão expirou!" Visible="false" /></td>
                </tr>
				<tr>
					<td>Cartão:</td>
					<td><asp:TextBox ID="txtCartao" runat="server" TextMode="SingleLine" Width="200px" ToolTip="Informe o número de sua carteirinha, com os hífens. Ex: 60-89891-9090" /></td>
				</tr>
				<tr>
					<td>Data de Nascimento:</td>
					<td><asp:TextBox ID="txtNascimento" runat="server" Width="200px" CssClass="calendario"/></td>
				</tr>
				<tr>
					<td colspan="2" align="center"><asp:Button ID="btnLogin" runat="server" Text="Entrar" onclick="btnLogin_Click" /></td>
				</tr>

			</table>
		</fieldset>
		</div>
	</div>
    </div>
    <div id="dvResult" runat="server" visible="false" style="width: 1050px">
        <table width="80%" style="margin-left:100px">            
            <tr>
                <td>
                    <h2>Consulta de Procedimentos</h2>
                    <br />
                    <table width="100%">
                        <tr>
                            <td style="width:100px"><b>Código TUSS</b></td>
                            <td><asp:TextBox ID="txtCodigoTuss" runat="server" Width="100px" /></td>
                        </tr>
                        <tr>
                            <td><b>Nome TUSS</b></td>
                            <td><asp:TextBox ID="txtNomeTuss" runat="server" Width="200px" /></td>
                        </tr>
                        <tr>
                            <td colspan="2" style="text-align:center">
                                <asp:Button ID="btnBuscar" runat="server" Text="Consultar" OnClick="btnBuscar_Click" />
                                <asp:Button ID="btnExportar" runat="server" Text="Gerar PDF" OnClick="btnExportar_Click" Visible="false" />
                            </td>
                        </tr>
                    </table>
                    <asp:Label ID="lblContador" runat="server" />
                    <asp:GridView ID="gdvResultado" runat="server" AutoGenerateColumns="false" CssClass="tabela" Width="100%"
                        AllowPaging="true" AllowSorting="false" PagerSettings-Mode="NumericFirstLast" PagerSettings-PageButtonCount="10" 
                        PageSize="50" OnPageIndexChanging="gdvResultado_PageIndexChanging">
                        <Columns>
                            <asp:BoundField HeaderText="CÓDIGO TUSS" DataField="cd_mascara" />
                            <asp:BoundField HeaderText="TERMINOLOGIA DE PROCEDIMENTOS E EVENTOS EM SAÚDE (TUSS)" DataField="ds_servico" />
                            <asp:TemplateField HeaderText="ROL ANS">
                                <ItemTemplate>
                                    <%# Convert.ToString(Eval("cd_rol_ans")).Trim().Length == 0 ? "NÃO" : "SIM" %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="POSSUI COBERTURA">
                                <ItemTemplate>
                                    <%# Convert.ToInt32(Eval("has_plano")) == 0 ? "NÃO" : "SIM" %>
                                </ItemTemplate>
                            </asp:TemplateField>                            
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
