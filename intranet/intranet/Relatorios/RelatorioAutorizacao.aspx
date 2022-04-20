<%@ Page Title="Autorizações ISA" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="RelatorioAutorizacao.aspx.cs" Inherits="eVidaIntranet.Relatorios.RelatorioAutorizacao" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function ConfigControlsCustom() {
            $('#<%= txtCpf.ClientID %>').ForceNumericOnly();
            $('#<%= txtAutorizacaoIsa.ClientID %>').ForceNumericOnly();
            $('#<%= txtAutorizacaoWeb.ClientID %>').ForceNumericOnly();
		    $('#<%= txtMatricula.ClientID %>').ForceNumericOnly();

            $('#a').keyup(function() {
                FilterItems($('#a').val(), '#<%= dpdUsuario.ClientID %>');
            });
            
        }

        var arrValues = new Object();
        function FilterItems(valor, dropBoxId) {
            var ops = $(dropBoxId + ' option');
            if (!arrValues.hasOwnProperty(dropBoxId)) {
                var arr = new Array();
                
                arrValues[dropBoxId] = $.map(ops, function (option) {
                    return option;
                });
            }

            var allValues = arrValues[dropBoxId];
            $(dropBoxId).empty();
            ops = $(dropBoxId).find('option');
            for (var i = 0; i < allValues.length; i++) {
                var opt = allValues[i];
                if (opt.text.indexOf(valor) >= 0) {
                    ops.end().append('<option value="' + opt.value + '">' + opt.text + '</option>')
                }
            }
        }
	</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    <table width="850px" cellspacing="10px">
	<tr>
		<td colspan="4"><h2 class="componentheading">Filtros</h2></td>
	</tr>
	<tr>
		<td style="width:140px; text-align: right">Período:</td>
		<td align="left">de <asp:TextBox ID="txtInicio" runat="server" Width="80px" MaxLength="10" CssClass="calendario"  /> até 
                <asp:TextBox ID="txtFim" runat="server" Width="80px" MaxLength="10" CssClass="calendario" /></td>
        <td style="width:160px; text-align: right">Matrícula:</td>
        <td><asp:TextBox ID="txtMatricula" runat="server" Width="100%" MaxLength="10"/></td>
	</tr>
    <tr>
		<td style="text-align: right">Nome do Titular:</td>
		<td align="left" colspan="3">
            <asp:Panel ID="pnlTitular" runat="server" DefaultButton="imgBuscaTitular">
            <asp:TextBox ID="txtFTitular" runat="server" Width="120px" MaxLength="60"  />
            <asp:ImageButton ID="imgBuscaTitular" runat="server" OnClick="imgBuscaTitular_Click" ImageAlign="Middle"  ImageUrl="~/img/seta_direita.png" Width="30px"/>
            <asp:DropDownList ID="dpdTitular" runat="server" DataTextField="NM_TITULAR" DataValueField="NM_TITULAR" Width="511px">
                <asp:ListItem Text="TODOS" Value="" />
            </asp:DropDownList>
                </asp:Panel>
		</td>        
	</tr>
    <tr>
		<td style="text-align: right">Nome do Dependente:</td>
		<td align="left" colspan="3">
            <asp:Panel ID="pnlDependente" runat="server" DefaultButton="imgBuscaDependente">
            <asp:TextBox ID="txtFDependente" runat="server" Width="120px" MaxLength="60"  />
            <asp:ImageButton ID="imgBuscaDependente" runat="server" OnClick="imgBuscaDependente_Click" ImageAlign="Middle" ImageUrl="~/img/seta_direita.png" Width="30px"/>
            <asp:DropDownList ID="dpdDependente" runat="server" DataTextField="NM_BENEFICIARIO" DataValueField="NM_BENEFICIARIO" Width="511px">
                <asp:ListItem Text="TODOS" Value="" />
            </asp:DropDownList>
            </asp:Panel>
		</td>        
	</tr>
    <tr>
		<td style="text-align: right">Nº Autorização ISA:</td>
		<td align="left"><asp:TextBox ID="txtAutorizacaoIsa" runat="server" Width="120px" MaxLength="8"  /></td>
        <td style="text-align: right">Nº Autorização WEB:</td>
		<td align="left"><asp:TextBox ID="txtAutorizacaoWeb" runat="server" Width="100%" MaxLength="8"  /></td>        
	</tr>
    <tr>
		<td style="text-align: right">Tipo :</td>
		<td align="left"><asp:DropDownList ID="dpdTipo" runat="server" Width="120px" >
            <asp:ListItem Text="Todos" Value="" />
            <asp:ListItem Value="ODONTO" Text="ATENDIMENTO ODONTOLÓGICO" />
            <asp:ListItem Value="INTERN" Text="INTERNAÇÃO HOSPITALAR" />
            <asp:ListItem Value="PROC" Text="PROCEDIMENTOS ESPECIALIZADOS" />
            <asp:ListItem Value="CART" Text="CARTA COMPROMISSO" />
            <asp:ListItem Value="PSICO" Text="PSICOLOGIA" />
            </asp:DropDownList></td>
        <td style="text-align: right">Status:</td>
		<td align="left"><asp:DropDownList ID="dpdStatus" runat="server" Width="100%" >
            <asp:ListItem Text="Todos" Value="" />
            <asp:ListItem Value="A" Text="APROVADA" />
            <asp:ListItem Value="C" Text="CANCELADA" />
            <asp:ListItem Value="E" Text="EM PERICIA" />
            <asp:ListItem Value="P" Text="PENDENTE" />
            <asp:ListItem Value="R" Text="REPROVADA" />
            <asp:ListItem Value="U" Text="UTILIZADA" />
            </asp:DropDownList></td>
	</tr>
    <tr>
		<td style="text-align: right">Código do Serviço:</td>
		<td align="left"><asp:TextBox ID="txtCodServico" runat="server" Width="120px" MaxLength="20"  /></td>
        <td style="text-align: right">Sistema de Atendimento :</td>
		<td align="left"><asp:DropDownList ID="dpdSistema" runat="server" Width="100%" >
            <asp:ListItem Text="Todos" Value="" />
            <asp:ListItem Value="CRED" Text="Credenciamento" />
            <asp:ListItem Value="REEMB" Text="Reembolso" />
            </asp:DropDownList></td>        
	</tr>
    <tr>
        <td style="text-align: right">Descrição do Serviço:</td>
		<td align="left" colspan="3">
            <asp:Panel ID="pnlDesServico" runat="server" DefaultButton="imgBuscaDesServico">
            <asp:TextBox ID="txtFDesServico" runat="server" Width="120px" MaxLength="60"  />
            <asp:ImageButton ID="imgBuscaDesServico" runat="server" OnClick="imgBuscaDesServico_Click" ImageAlign="Middle" ImageUrl="~/img/seta_direita.png" Width="30px"/>
            <asp:DropDownList ID="dpdDesServico" runat="server" DataTextField="BR8_DESCRI" DataValueField="BR8_DESCRI" Width="511px">
                <asp:ListItem Text="TODOS" Value="" />
            </asp:DropDownList>
            </asp:Panel>
		</td>
    </tr>
    <tr>		
        <td style="text-align: right">CPF/CNPJ Credenciado:</td>
		<td align="left"><asp:TextBox ID="txtCpf" runat="server" Width="120px" MaxLength="14" /></td>  
        <td style="text-align: right">Usuário:</td>
        <td align="left"><asp:DropDownList ID="dpdUsuario" runat="server" Width="100%" MaxLength="30" DataTextField="USER_UPDATE" DataValueField="USER_UPDATE" /></td>              
	</tr>
    <tr>
		<td style="text-align: right">Nome do Credenciado:</td>
		<td align="left" colspan="3">
            <asp:Panel ID="pnlCredenciado" runat="server" DefaultButton="imgBuscaCredenciado">
            <asp:TextBox ID="txtFCredenciado" runat="server" Width="120px" MaxLength="80"  />
            <asp:ImageButton ID="imgBuscaCredenciado" runat="server" OnClick="imgBuscaCredenciado_Click" ImageAlign="Middle" ImageUrl="~/img/seta_direita.png" Width="30px"/>
            <asp:DropDownList ID="dpdCredenciado" runat="server" DataTextField="NM_RAZAO_SOCIAL" DataValueField="NM_RAZAO_SOCIAL" Width="511px">
                <asp:ListItem Text="TODOS" Value="" />
            </asp:DropDownList>
            </asp:Panel>
		</td>
	</tr>
    <tr>
		<td colspan="4" style="text-align:center"><asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="btnBuscar_Click" />
            <asp:Button ID="btnExportar" runat="server" Text="Exportar" OnClick="btnExportar_Click" Visible="false" />
		</td>
	</tr>
    </table>

    <table>
	<tr style="height:300px">
		<td>
            <asp:Label ID="lblCount" runat="server" />
			<asp:GridView ID="gdvRelatorio" runat="server" AutoGenerateColumns="false"
				AllowSorting="false" CssClass="tabela" Width="3800px" OnSorting="gdvRelatorio_Sorting" 
                AllowPaging="True" PageSize="50" PagerSettings-PageButtonCount="20" PagerSettings-Position="TopAndBottom" onpageindexchanging="gdvRelatorio_PageIndexChanging" >
                <Columns>
					<asp:BoundField HeaderText="MATRICULA" DataField="cd_funcionario" SortExpression="cd_funcionario" DataFormatString="{0:0}" />
					<asp:BoundField HeaderText="NOME TITULAR" DataField="nm_titular" SortExpression="nm_titular" />
					<asp:BoundField HeaderText="NOME BENEFICIARIO" DataField="nm_beneficiario" SortExpression="nm_beneficiario" />
                    <asp:BoundField HeaderText="Nº AUTORIZACAO ISA" DataField="nr_autorizacao" SortExpression="nr_autorizacao" DataFormatString="{0:0}"/>
                    <asp:BoundField HeaderText="Nº AUTORIZACAO WEB" DataField="nr_autorizacao_web" SortExpression="nr_autorizacao_web" DataFormatString="{0:0}"/>
                    <asp:BoundField HeaderText="DATA REGISTRO" DataField="dt_registro" SortExpression="dt_registro" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:BoundField HeaderText="DATA DA AUTORIZACAO" DataField="dt_autorizacao" SortExpression="dt_autorizacao" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:BoundField HeaderText="DATA VALIDADE" DataField="dt_validade" SortExpression="dt_validade" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:BoundField HeaderText="TIPO DE AUTORIZACAO" DataField="tp_autorizacao" SortExpression="tp_autorizacao" />
                    <asp:BoundField HeaderText="DIAS AUTORIZADOS" DataField="nr_dias_autorizados" SortExpression="nr_dias_autorizados" DataFormatString="{0:0}" />
                    <asp:BoundField HeaderText="STATUS AUTORIZACAO" DataField="st_autorizacao" SortExpression="st_autorizacao" />
                    <asp:BoundField HeaderText="MOTIVO CANCELAMENTO" DataField="ds_motivo_cancelamento" SortExpression="ds_motivo_cancelamento" />
                    <asp:BoundField HeaderText="MOTIVO REPROVACAO" DataField="ds_compl_reprovacao" SortExpression="ds_compl_reprovacao" />
                    <asp:BoundField HeaderText="SISTEMA DE ATENDIMENTO" DataField="tp_sistema_atend" SortExpression="tp_sistema_atend" />
                    <asp:BoundField HeaderText="DATA INICIO INTERNACAO" DataField="dt_inicio_autorizacao" SortExpression="dt_inicio_autorizacao" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:BoundField HeaderText="DATA FIM INTERNACAO" DataField="dt_termino_autorizacao" SortExpression="dt_termino_autorizacao" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:BoundField HeaderText="OBSERVACAO" DataField="ds_observacao" SortExpression="ds_observacao" />
                    <asp:BoundField HeaderText="CODIGO SERVICO" DataField="cd_mascara" SortExpression="cd_mascara" />
                    <asp:BoundField HeaderText="DESCRICAO" DataField="ds_servico" SortExpression="ds_servico" />                    
                    <asp:BoundField HeaderText="QTD SOLICITADA" DataField="qt_solicitada" SortExpression="qt_solicitada" DataFormatString="{0:0}" />
                    <asp:BoundField HeaderText="QTD AUTORIZADA" DataField="qt_autorizada" SortExpression="qt_autorizada" DataFormatString="{0:0}" />
                    <asp:BoundField HeaderText="RAZAO SOCIAL" DataField="nm_razao_social" SortExpression="nm_razao_social" />
                    <asp:BoundField HeaderText="CPF/CNPJ" DataField="nr_cnpj_cpf" SortExpression="nr_cnpj_cpf" />
                    <asp:BoundField HeaderText="MUNICIPIO" DataField="ds_municipio" SortExpression="ds_municipio" />
                    <asp:BoundField HeaderText="ESTADO" DataField="cd_uf" SortExpression="cd_uf" />
                    <asp:BoundField HeaderText="MEDICO ASSISTENTE" DataField="nm_medico_solicitante" SortExpression="nm_medico_solicitante" />
                    <asp:BoundField HeaderText="Nº CONSELHO" DataField="cd_profissional" SortExpression="cd_profissional" DataFormatString="{0:0}" />
                    <asp:BoundField HeaderText="CONSELHO" DataField="cd_tipo_registro" SortExpression="cd_tipo_registro" />
                    <asp:BoundField HeaderText="USUÁRIO" DataField="user_update" SortExpression="user_update" />
				</Columns>
			</asp:GridView>
		</td>
	</tr>

</table>
    
</asp:Content>
