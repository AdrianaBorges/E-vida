<%@ Page Title="Canal Gestante" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="CanalGestante.aspx.cs" Inherits="eVidaIntranet.Admin.CanalGestante" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">

        var POP_CREDENCIADO = 4;
        var POP_PROFISSIONAL = 5;
        
        function ConfigControlsCustom() {
            createLocator(650, 550, dlgOpen, null, dlgCallback);

            $('.inteiro').each(function () {
                $('#' + this.id).ForceNumericOnly();
            });

            if (typeof (configAllCounters) === 'function')
                setTimeout(function () { configAllCounters(); }, 1000);
        }

        function dlgOpen(handler, ev, ui) {
            var src = "";
            switch (handler.tipo) {
                case POP_CREDENCIADO: src = '../GenPops/PopCredenciado.aspx?enableEmpty=false'; break;
                case POP_PROFISSIONAL: src = '../GenPops/PopProfissional.aspx?enableEmpty=false'; break;
            }
            setLocatorUrl(src);
        }

        function dlgCallback(handler, response) {
            if (handler.tipo == POP_CREDENCIADO)
                $("#<%: hidCodCredenciado.ClientID%>").val(response);
            else if (handler.tipo == POP_PROFISSIONAL)
                $("#<%: hidCodProfissional.ClientID%>").val(response);

            executeLocatorHandlerPost(handler);
        }

        function openPopCredenciado(btnLoc) {
            defaultOpenPop(POP_CREDENCIADO, btnLoc, 0, 0, "Buscar credenciado");
            return false;
        }
        function openPopProfissional(btnLoc) {
            defaultOpenPop(POP_PROFISSIONAL, btnLoc, 0, 0, "Buscar Médico");
            return false;
        }
        function confirmExclusao() {
            return confirm("Deseja realmente remover o item?");
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    <table width="800px" style="margin-left:50px;">
        <tr>
            <td>
                <table width="100%" style="border-collapse: collapse; border-style:solid">
                    <tr>            
                        <td style="padding:10px"><b>Ano:</b></td>
                        <td><asp:DropDownList ID="dpdAno" runat="server" AutoPostBack="true" OnSelectedIndexChanged="dpdAno_SelectedIndexChanged" /></td>
                        <td><asp:Button ID="btnBuscar" runat="server" OnClick="btnBuscar_Click" Text="Buscar" /></td>
                    </tr>
                </table>

            </td>
        </tr>
        <tr>
            <td>
                
                <asp:Panel ID="pnlDados" runat="server" Visible="false">
                    <table class="tabela" width="100%">
                        <tr>
                            <th>Operadora</th>
                            <th>% Parto Normal</th>
                        </tr>
                        <tr>
                            <td>E-VIDA</td>
                            <td><asp:TextBox ID="txtPartoNormal" runat="server" MaxLength="6" Width="100px" />%</td>
                        </tr>
                    </table>
                    <br />
                    <table class="tabelaForm" width="100%">
                        <tr>
                            <td>
                                <h2>Estabelecimentos
                                <asp:Button ID="btnAddEstabelecimento" runat="server" Text="Adicionar" OnClientClick="return openPopCredenciado(this);" OnClick="btnAddEstabelecimento_Click" /></h2>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:HiddenField ID="hidCodCredenciado" runat="server" />
                                <asp:ListView ID="ltvEstabelecimentos" runat="server" ItemPlaceholderID="row"
			                        ItemType="eVidaIntranet.Admin.CanalGestante.CredenciadoScreen" DataKeyNames="Key">
			                        <EmptyDataTemplate><p style="text-align:center"><b>Não existem estabelecimentos associados.</b></p></EmptyDataTemplate>
			                        <LayoutTemplate>
				                        <table class="tabela" style="width:800px">
					                        <tr>
						                        <th style="width:450px">Nome</th>	
						                        <th style="width:90px">UF</th>
						                        <th>% Parto Normal</th>
                                                <th></th>
					                        </tr>
					                        <tr id="row" runat="server"></tr>
				                        </table>
			                        </LayoutTemplate>
			                        <ItemTemplate>
				                        <tr>
					                        <td><b><%# Item.Nome %></b></td>
					                        <td><%# Item.Config.Uf %></td>
                                            <td><asp:TextBox ID="txtPartoNormal" runat="server" Width="100px" MaxLength="6" Text='<%# Item.Config.PartoNormal.ToString("##0.##") %>' /></td>
					                        <td><asp:ImageButton ID="bntExcluirEstabelecimento" runat="server" ImageUrl="~/img/remove.png" Height="20px" 
							                        OnClick="bntExcluirEstabelecimento_Click" OnClientClick='<%# "return confirmExclusao();" %>' />
					                        </td>
				                        </tr>
			                        </ItemTemplate>
		                        </asp:ListView>        
                            </td>
                        </tr>
                    </table>
                    <br />
                    <table class="tabelaForm" width="100%">
                        <tr>
                            <td>
                                <h2>Médicos
                                <asp:Button ID="btnAddMedico" runat="server" Text="Adicionar" OnClientClick="return openPopProfissional(this);" OnClick="btnAddMedico_Click" /></h2>

                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:HiddenField ID="hidCodProfissional" runat="server" />
                                <asp:ListView ID="ltvMedicos" runat="server" ItemPlaceholderID="row"
			                        ItemType="eVidaIntranet.Admin.CanalGestante.ProfissionalScreen" DataKeyNames="Key"
                                    OnItemDataBound="ltvMedicos_ItemDataBound">
			                        <EmptyDataTemplate><p style="text-align:center"><b>Não existem médicos associados.</b></p></EmptyDataTemplate>
			                        <LayoutTemplate>
				                        <table class="tabela" style="width:800px">
					                        <tr>
						                        <th style="width:450px">Nome</th>	
						                        <th style="width:90px">CRM/UF</th>
						                        <th>% Parto Normal</th>
                                                <th></th>
					                        </tr>
					                        <tr id="row" runat="server"></tr>
				                        </table>
			                        </LayoutTemplate>
			                        <ItemTemplate>
				                        <tr>
					                        <td><b><%# Item.Nome %></b></td>
					                        <td><%# Item.Config.Codsig + " / " + Item.Config.Estado %></td>
                                            <td><asp:TextBox ID="txtPartoNormal" runat="server" Width="100px" MaxLength="6" Text='<%# Item.Config.PartoNormal.ToString("##0.##") %>' /></td>
					                        <td><asp:ImageButton ID="bntExcluirMedico" runat="server" ImageUrl="~/img/remove.png" Height="20px" 
							                        OnClick="bntExcluirMedico_Click" OnClientClick='<%# "return confirmExclusao();" %>' />
					                        </td>
				                        </tr>
			                        </ItemTemplate>
		                        </asp:ListView>    
                            </td>
                        </tr>
                    </table>
                    <br />
                    <table width="100%">
                        <tr>
                            <td align="center"><asp:Button ID="btnSalvar" runat="server" Text="Salvar" OnClick="btnSalvar_Click" /></td>
                        </tr>
                    </table>
                </asp:Panel>
            </td>
        </tr>
    </table>

</asp:Content>
