<%@ Page Title="Indisponibilidade de Rede" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="IndisponibilidadeRede.aspx.cs" Inherits="eVidaBeneficiarios.Forms.IndisponibilidadeRede" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">

        var POP_ARQUIVO = 2;
        var POP_CREDENCIADO = 4;
        
        function ConfigControlsCustom() {
            createLocator(650, 550, dlgOpen, null, dlgCallback);
            setMaskCelular('<%= txtTelContato.ClientID %>');

            var formatPreco = {
                prefix: '',
                centsSeparator: ',',
                thousandsSeparator: '.',
                centsLimit: 2,
                clearPrefix: true,
                allowNegative: false
            };

            $('#<%= txtValor.ClientID %>').priceFormat(formatPreco);

        }
        function dlgOpen(handler, ev, ui) {
            var src = "";
            switch (handler.tipo) {
                case POP_ARQUIVO: src = '../GenPops/PopUpload.aspx?tipo=<%: eVidaGeneralLib.Util.UploadFilePrefix.INDISPONIBILIDADE_REDE.Value %>'; break;
            }
            setLocatorUrl(src);
        }

        function dlgCallback(handler, response) {
            switch (handler.tipo) {
                case POP_ARQUIVO: break;
            }
        }

        function onAfterUpload(url, originalName) {
            $("#<%:hidArqFisico.ClientID%>").val(url);
            $("#<%:hidArqOrigem.ClientID%>").val(originalName);
            closeLocator();
            <%= ClientScript.GetPostBackEventReference(btnIncluirArquivo, "") %>
        }


        function openArquivo() {
            var handler = new LocatorHandler(POP_ARQUIVO);
            openLocator("Arquivos", handler);
            return false;
        }

        function openDownload(idIndisponibilidade, fId, isNew) {
            if (isNew) {
                alert('Arquivos Novos não podem ser visualizados. O formulário deve ser salvo para disponibilizar a visualização!');
                return false;
            }
            openFile('<%= eVidaGeneralLib.Util.FileUtil.FileDir.INDISPONIBILIDADE_REDE %>', "ID=" + idIndisponibilidade + ";" + fId);
            return false;
        }
                
        function confirmExclusaoArquivo() {
            return confirm("Deseja realmente retirar este arquivo?");
        }

        function showExistente(id) {
            var conf = confirm("Já existe uma solicitação para este beneficiário. Deseja ir à solicitação já criada?");
            if (conf) {
                window.location = 'IndisponibilidadeRede.aspx?ID=' + id;
                return false;
            }
            return false;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    <div id="conteudo">
        <asp:HiddenField ID="litProtocolo" runat="server" />
        <div id="dvMensagemAlteracao" runat="server" visible="false">
            <b>Após cadastro da solicitação, você não pode alterá-la.
            Entre em contato com o setor de credenciamento.</b>
        </div>
        <h2>1 - Informações Gerais</h2>
	    <table id="tbInfoGeral" border="1" class="tabelaForm" style="width:900px">
            <tr>
                <td>
				    <b>Protocolo</b><br />
				    <asp:Label ID="lblProtocolo" runat="server" Font-Bold="true"/>
                </td>
                <td>
				    <b>Protocolo ANS</b><br />
				    <asp:Label ID="lblProtocoloAns" runat="server" Font-Bold="true"/>
                </td>
                <td>
                    <b>Data de Solicitação</b><br />
				    <asp:Label ID="lblDataSolicitacao" runat="server"/>
                </td>                
            </tr>
            <tr>
                <td colspan="2">
				    <b>Situação</b><br />
				    <asp:Label ID="lblSituacao" runat="server"/>
                </td>
                <td>
                    <b>Prazo Estimado</b><br />
                    <asp:Label ID="lblPrazo" runat="server" />
                    <asp:Label ID="lblAtraso" runat="server" ForeColor="Red" Font-Bold="true" Visible="false" Text=" - EM ATRASO" />
                </td>                
            </tr>            
        </table>
        <h2>2 - Dados do Beneficiário</h2>
	    <table id="tbBeneficiario" border="1" class="tabelaForm" style="width:900px">
            <tr>
                <td colspan="2">
				    <asp:Label ID="lblDependente" runat="server" Text="Nome do Beneficiário *" Font-Bold="true" /><br />
				    <asp:DropDownList ID="dpdBeneficiario" runat="server" DataValueField="Cdusuario" DataTextField="Nomusr" OnSelectedIndexChanged="dpdBeneficiario_SelectedIndexChanged" AutoPostBack="true" Width="600px" />
                </td>
                <td>
                    <b>Telefone de Contato *</b><br />
                    <asp:TextBox ID="txtTelContato" runat="server" Width="180px" MaxLength="20" />
                </td>
            </tr>
            <tr>
                <td>
                    <b>E-mail *</b><br />
                    <asp:TextBox ID="txtEmail" runat="server" Width="380px" MaxLength="200" />
                </td>
                <td><b>UF *</b><br />
                    <asp:DropDownList ID="dpdUf" runat="server" Width="120px" OnSelectedIndexChanged="dpdUf_SelectedIndexChanged" AutoPostBack="true" />
                </td>
                <td><b>Município *</b><br />
                    <asp:DropDownList ID="dpdMunicipio" runat="server" Width="300px" />
                </td>
            </tr>
        </table>
        <br />
        
        <h2>3 - Dados da Solicitação</h2>
	    <table id="tbSolicitacao" runat="server" border="1" class="tabelaForm" style="width:900px">
            <tr>
                <td colspan="2">
                    <b>Especialidade *</b><br />
                    <asp:DropDownList ID="dpdEspecialidade" runat="server" DataValueField="Id" DataTextField="Nome" OnSelectedIndexChanged="dpdEspecialidade_SelectedIndexChanged" AutoPostBack="true" />
                </td>
                <td>
                    <b>Serviço *</b><br />
                    <asp:DropDownList ID="dpdPrioridade" runat="server" Width="350px" />
                </td>
            </tr>
            <tr>
				<td>
                    <asp:Label ID="lblCpfCnpj" runat="server" Text="CPF/CNPJ" Font-Bold="true" /><br />
				    <asp:TextBox ID="txtCpfCnpj" runat="server" Width="150px" />
                </td>
			    <td>
				    <asp:Label ID="lblRazaoSocial" runat="server" Text="Nome/Razão Social" Font-Bold="true" /><br />
				    <asp:TextBox ID="txtRazaoSocial" runat="server" Width="350px"/>
                </td>
                <td>
                    <b>Valor:<br /></b>
                    <asp:TextBox ID="txtValor" runat="server" Width="200px" />
                </td>
			</tr>
            <tr>
                <td colspan="3" align="center">
                    <hr />
                    <b>Observação</b><br />
                    <asp:GridView ID="gdvObs" runat="server" AutoGenerateColumns="false" Width="700px" Visible="false" CssClass="tabela">
                        <Columns>
                            <asp:BoundField HeaderText="Data" DataField="DataRegistro" DataFormatString="{0:dd/MM/yyyy HH:mm}" ItemStyle-Width="150px"/>
                            <asp:BoundField HeaderText="Observação" DataField="Observacao" />
                        </Columns>
                    </asp:GridView><br />
                    <asp:TextBox ID="txtObs" runat="server" Width="700px" TextMode="MultiLine"/><br />
                    <asp:Button ID="btnSalvarObs" runat="server" Text="Incluir Observação" OnClick="btnSalvarObs_Click" Visible="false" />
                </td>
            </tr>            
            <tr>
                <td align="center" colspan="3"><asp:Button ID="btnSalvar" runat="server" Text="Salvar alterações" OnClick="btnSalvar_Click" /></td>
            </tr>
            
	    </table>
        <br />
        
        <h2>4 - Anexos</h2>
        
        <asp:UpdatePanel runat="server" ID="updArquivos" UpdateMode="Conditional">
			<ContentTemplate>
				<div id="dvArquivos" runat="server">
					<asp:HiddenField ID="hidArqFisico" runat="server" />
					<asp:HiddenField ID="hidArqOrigem" runat="server" />

	                <asp:ListView ID="ltvArquivo" runat="server" ItemPlaceholderID="contentID" ItemType="eVidaGeneralLib.VO.ArquivoTelaVO">
			            <LayoutTemplate>
				            <table class="tabela" >
					            <tr>
						            <th></th>
						            <th>Nome</th>						
					            </tr>
					            <tr id="contentID" runat="server"/>
				            </table>
			            </LayoutTemplate>
			            <ItemTemplate>
				            <tr>
					            <td><asp:ImageButton ID="btnRemoverArquivo" runat="server" 
						            ImageUrl="~/img/remove.png" OnClick="bntRemoverArquivo_Click" 
						            CommandArgument='<%# Item.NomeFisico %>'
						            Visible='<%# Item.IsNew || btnIncluirArquivo.Visible %>'/></td>
					            <td>
                                    <a href="javascript:void(0)" onclick="return openDownload('<%# Id %>','<%# Item.Id %>', <%# Item.IsNew.ToString().ToLower() %>);">								
						            <%# Item.NomeTela %><%# Item.IsNew ? " - Novo " : "" %>
						            </a></td>
				            </tr>
			            </ItemTemplate>
                        <EmptyDataTemplate>
                            Não existem anexos na solicitação.<br />
                        </EmptyDataTemplate>
		            </asp:ListView>
					<asp:Button ID="btnIncluirArquivo" runat="server" Text="Incluir arquivo" OnClientClick="return openArquivo()" OnClick="btnIncluirArquivo_Click"  UseSubmitBehavior="false"/>
                </div>
			</ContentTemplate>
		</asp:UpdatePanel>
        <br />

         <table width="50%">
            <tr>
                <td align="center"><asp:Button ID="btnEnviar" runat="server" Text="Enviar" OnClick="btnSalvar_Click" /></td>
            </tr>
        </table>
    </div>
</asp:Content>
