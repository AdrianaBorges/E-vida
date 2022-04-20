<%@ Page Title="Canal Gestante" Language="C#" MasterPageFile="~/Internal/ExternalPages.Master" AutoEventWireup="true" CodeBehind="CanalGestante.aspx.cs" Inherits="eVidaBeneficiarios.CanalGestante.CanalGestante" %>
<%@ Register TagPrefix="evida" TagName="Header" Src="~/CanalGestante/Header.ascx" %>
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
    <script type="text/javascript">
        var POP_ESCLARECIMENTOS = 1;
        var idProtocolo;
        function ConfigControlsCustom() {
            createLocator(650, 550, dlgOpen, dlgClose, dlgCallback);
        }

        function dlgOpen(handler, ev, ui) {
            var src = "";
            switch (handler.tipo) {
                case POP_ESCLARECIMENTOS: src = './PopSolEsclarecimento.aspx?ID=' + idProtocolo; break;
            }
            setLocatorUrl(src);
        }

        function dlgCallback(handler, response) {
            closeLocator();
            switch (handler.tipo) {
                case POP_ESCLARECIMENTOS:
                    break;
            }
        }
        function dlgClose() {
            document.getElementById("<%=btnRefresh.ClientID%>").click();
        }
        function openLocalPdf(fid, pid) {
            window.open('./DownloadFile.aspx?FID=' + fid + '&PID=' + pid, '_info_', "", true);
        }

        function openProtocolo(pid) {
            openLocalPdf('PROTOCOLO', pid);
            if (confirm("Deseja esclarecimentos adicionais?")) {
                idProtocolo = pid;
                defaultOpenPop(POP_ESCLARECIMENTOS, document.getElementById('<%= btnRefresh.ClientID %>'), null, null, "Esclarecimentos Adicionais");
            }
            return false;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    <evida:Header id="header" runat="server" /><br />
    <table width="80%" style="margin-left:100px">
        <tr>
            <td>

                <table width="100%" class="tbInfoAdicional" border="1">
                    <tr>
                        <td colspan="3"><h2>Informações para contato</h2></td>
                    </tr>
                    <tr>
                        <td><b>E-mail</b><br />
                            <asp:TextBox ID="txtEmail" runat="server" Width="400px"/>
                        </td>
                        <td><b>Telefone</b><br />
                            <asp:TextBox ID="txtTelefone" runat="server" Width="200px" />
                        </td>
                        <td>
                            <asp:Button ID="btnSalvar" runat="server" Text="Salvar Dados" OnClick="btnSalvar_Click" />
                        </td>
                    </tr>
                </table>

                
            </td>
        </tr>
        <tr>
            <td>
                <br />
                <p>A Resolução Normativa n° 368 de 06 de janeiro de 2015 da Agência Nacional de Saúde Suplementar (ANS) prevê medidas de estímulo ao parto normal e garante o direito de acesso à informação das beneficiárias de planos de saúde aos percentuais de cirurgias cesáreas e de partos normais por operadora, por estabelecimento de saúde e por médico. Garante também o direito de acesso à informação sobre o uso do partograma, do cartão da gestante e da carta de informação à gestante no âmbito da saúde suplementar.</p>
                <p>A E-VIDA criou este canal para viabilizar o fornecimentos das informações, bem como a disponibilização do Cartão da Gestante e do Partograma.</p>
            </td>
        </tr>
        <tr>
            <td align="center">
                <br />
                <p>AS INFORMAÇÕES REFEREM-SE AOS DADOS VINCULADOS APENAS AOS PARTOS EFETUADOS PELA OPERADORA, E NÃO AO TOTAL DE PARTOS REALIZADOS PELOS MÉDICOS OU ESTABELECIMENTOS COOPERADOS, CREDENCIADOS OU REFERENCIADOS A MAIS DE UMA OPERADORA.</p>
                <br />
                <asp:Button ID="btnCartaoInfo" runat="server" Text="Carta de Informação" OnClick="btnCartaoInfo_Click" />&nbsp;
                <asp:Button ID="btnCartaoGest" runat="server" Text="Cartão Gestante" OnClick="btnCartaoGest_Click" />&nbsp;
                <asp:Button ID="btnPartograma" runat="server" Text="Partograma" OnClick="btnPartograma_Click" />&nbsp;                
            </td>
        </tr>
        <tr>
            <td>
                <br /><hr />
                <table width="100%">
                    <tr>
                        <td colspan="2"><p>Selecione quais dados você quer obter referente aos percentuais de parto normal e cirurgia cesárea.
                            <br />Somente serão mostrados os Estabelecimentos ou Médicos que efetivamente realizaram parto através da E-VIDA. <br />
                            Caso não apareça o nome de um estabelecimento ou médico é por que não realizaram parto no período.</p></td>
                    </tr>
                    <tr>
                        <td style="width:120px"><p class="p2"><b>Operadora</b></p></td>
                        <td><asp:DropDownList ID="dpdOperadora" runat="server" Width="200px">
                            <asp:ListItem Text="E-VIDA" Value="EV" />
                         </asp:DropDownList></td>
                    </tr>
                    <tr>
                        <td><p class="p2"><b>UF</b></p></td>
                        <td><asp:DropDownList ID="dpdUf" runat="server" OnSelectedIndexChanged="dpdUf_SelectedIndexChanged" AutoPostBack="true" /></td>
                    </tr>
                    <tr>
                        <td><p class="p2"><b>Estabelecimento</b></p></td>
                        <td><asp:DropDownList ID="dpdEstabelecimento" runat="server" DataValueField="Codigo" DataTextField="Nome" /></td>
                    </tr>
                    <tr>
                        <td><p class="p2"><b>Médico(a)</b></p></td>
                        <td><asp:DropDownList ID="dpdMedico" runat="server" DataValueField="Codigo" DataTextField="Nome" /></td>
                    </tr>
                    <tr>
                        <td colspan="2" align="center"><asp:Button ID="btnSolicitar" runat="server" Text="Solicitar Dados" OnClick="btnSolicitar_Click" /></td>
                    </tr>
                </table>
                
                <hr /><br />
            </td>
        </tr>
    </table>
    <table width="100%" style="margin-left:100px">
        <tr>
            <td align="center">
                <asp:Button ID="btnRefresh" runat="server" Text="Atualizar solicitações anteriores" OnClick="btnRefresh_Click" CssClass="oculto" />
                <h2>Solicitações Anteriores</h2>
                <asp:GridView ID="gdvSolAnterior" runat="server" CssClass="tabela" Width="1300px" AutoGenerateColumns="false" 
                    OnRowDataBound="gdvSolAnterior_RowDataBound" DataKeyNames="Id"
                    EmptyDataRowStyle-HorizontalAlign="Center">
                    <EmptyDataTemplate>Não existem solicitações anteriores.</EmptyDataTemplate>
                    <Columns>
                        <asp:TemplateField HeaderText="Protocolo">
                            <ItemTemplate>
                                <asp:Literal ID="ltProtocolo" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField HeaderText="Data de Solicitação" DataField="DataSolicitacao" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                        <asp:TemplateField HeaderText="Status">
                            <ItemTemplate>
                                <asp:Literal ID="ltStatus" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField HeaderText="Esclarecimentos Adicionais" DataField="Pendencia"/>
                        <asp:BoundField HeaderText="Resposta" DataField="Resposta"/>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="btnRegerar" runat="server" ImageUrl="~/img/PDF.png" Height="25px" OnClick="btnRegerar_Click" CommandArgument='<%# Eval("Id") %>' />
                                <asp:ImageButton ID="btnEmail" runat="server" ImageUrl="~/img/email.png" Height="25px" OnClick="btnEmail_Click" CommandArgument='<%# Eval("Id") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
    </table>
</asp:Content>
