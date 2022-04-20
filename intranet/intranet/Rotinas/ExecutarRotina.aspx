<%@ Page Title="Executar Rotina" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="ExecutarRotina.aspx.cs" Inherits="eVidaIntranet.Rotinas.ExecutarRotina" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    <table width="850px" cellspacing="10px">
        <tr>
            <td><b>Rotina:</b></td>
            <td><asp:DropDownList ID="dpdRotina" runat="server" DataValueField="Id" DataTextField="Nome"
                OnSelectedIndexChanged="dpdRotina_SelectedIndexChanged" AutoPostBack="true" Width="500px" /></td>
        </tr>
        <tr>
            <td><b>Nome:</b></td>
            <td><asp:Label ID="lblNome" runat="server" /></td>
        </tr>
        <tr>
            <td><b>Descrição:</b></td>
            <td><asp:Label id="lblDescricao" runat="server" /></td>
        </tr>
        <tr>
            <td colspan="2" style="text-align:center">
                <asp:Button ID="btnExecutar" runat="server" Text="Solicitar Execução" OnClick="btnExecutar_Click" Visible="false" />
                <asp:Button ID="btnAtualizar" runat="server" Text="Atualizar Histórico" OnClick="btnAtualizar_Click" Visible="false" />
		    </td>        
	    </tr>
    </table>
    <table width="950px">
	    <tr style="height:300px">
		    <td align="center">
                <div id="dvHistorico" runat="server" visible="false">
                    <h2>Histórico de execução da rotina</h2>
                    <br />
                    Atualizado em: <%= DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") %>
                </div>
                <asp:GridView ID="gdvRelatorio" runat="server" AutoGenerateColumns="false"
				    AllowSorting="false" CssClass="tabela" Width="900px" OnRowDataBound="gdvRelatorio_RowDataBound"
                    DataKeyNames="ID_EXEC" EmptyDataText="A rotina ainda não foi executada!" >
                <Columns>
					<asp:BoundField HeaderText="Data Solicitação" DataField="dt_criacao" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" />
					<asp:BoundField HeaderText="Usuário solicitante" DataField="nm_usuario" />
					<asp:BoundField HeaderText="Data Início" DataField="dt_inicio"  DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" />
                    <asp:BoundField HeaderText="Data Fim" DataField="dt_fim" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" />
                    <asp:BoundField HeaderText="Situação" DataField="st_registro" />
				</Columns>
			</asp:GridView>
            </td>
        </tr>
    </table>
</asp:Content>
