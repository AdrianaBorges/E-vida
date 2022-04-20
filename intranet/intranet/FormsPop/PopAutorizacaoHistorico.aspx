<%@ Page Title="" Language="C#" MasterPageFile="~/Internal/InternalPopup.Master" AutoEventWireup="true" CodeBehind="PopAutorizacaoHistorico.aspx.cs" Inherits="eVidaIntranet.FormsPop.PopAutorizacaoHistorico" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    <h1>Histórico de alterações da autorização: <asp:Literal ID="litProtocolo" runat="server" /></h1>
    <br />
    <asp:GridView ID="gdvHistorico" runat="server" CssClass="tabela" AutoGenerateColumns="false" Width="600px" OnRowDataBound="gdvRelatorio_RowDataBound">
        <Columns>
            <asp:BoundField HeaderText="Data/Hora" DataField="dt_alteracao" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" />
            <asp:BoundField HeaderText="Status" DataField="ST_AUTORIZACAO" />
            <asp:BoundField HeaderText="Origem" DataField="TP_ORIGEM_ALTERACAO" />
            <asp:BoundField HeaderText="Gestor" DataField="NM_USUARIO_ALTERACAO" />
        </Columns>
    </asp:GridView>
</asp:Content>
