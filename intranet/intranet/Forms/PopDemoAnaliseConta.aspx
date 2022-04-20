<%@ Page Title="Demonstrativo de Análise de Conta" Language="C#" MasterPageFile="~/Internal/InternalPopup.Master" AutoEventWireup="true" CodeBehind="PopDemoAnaliseConta.aspx.cs" Inherits="eVidaIntranet.Forms.PopDemoAnaliseConta" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">

    <table width="100%">
        <tr>
            <td>
                Referência: 
                <asp:DropDownList ID="dpdMes" runat="server" Width="150px" />
                <asp:DropDownList ID="dpdAno" runat="server" Width="100px" />
            </td>
        </tr>
        <tr>
            <td align="center"><br />
                <asp:Button ID="btnSolicitar" runat="server" Text="Solicitar" OnClick="btnSolicitar_Click" />
                <br />
            </td>
        </tr>
        <tr>
            <td>
                <br />
                <asp:GridView ID="gdvSolicitacoes" runat="server" CssClass="tabela" Width="95%" AutoGenerateColumns="false">
                    <EmptyDataTemplate>
                        Não existem solicitações para este CNPJ e Documento Fiscal
                    </EmptyDataTemplate>
                    <Columns>
                        <asp:BoundField HeaderText="CNPJ" DataField="CpfCnpj" />
                        <asp:BoundField HeaderText="Documento Fiscal" DataField="DocumentoFiscal" />
                        <asp:BoundField HeaderText="Referência" DataField="Referencia" DataFormatString="{0:MM/yyyy}" />
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
    </table>
</asp:Content>
