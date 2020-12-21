<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Areas.aspx.cs" Inherits="NCCSAN.Administracion.Areas" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .Popup_background
        {
            background-color: Gray;
            filter: alpha(opacity=70);
            opacity: 0.7;
        }
        
        .Popup_frontground
        {
            background-color: White;
            padding: 30px; /*border-style: dotted;
            border-width: 2px;*/
            box-shadow: 10px 10px 5px #555555;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="divTituloForm">
        <h1>
            Lista de Áreas</h1>
    </div>
    <asp:UpdatePanel ID="uPanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pnAreas" runat="server" HorizontalAlign="Center">
                <table class="tableControls" align="center" width="80%">
                    <tr>
                        <td class="tdUnique">
                            <asp:GridView ID="gvAreas" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                                HorizontalAlign="Center" Width="100%" ShowFooter="True" ShowHeaderWhenEmpty="True"
                                DataSourceID="SDSAreas" OnRowCommand="gvAreas_RowCommand">
                                <AlternatingRowStyle CssClass="rowDataAlt" />
                                <Columns>
                                    <asp:TemplateField HeaderText="Nombre área">
                                        <ItemTemplate>
                                            <asp:Label ID="lbNombre" runat="server" Text='<%#Eval("nombre_area") %>'></asp:Label>
                                        </ItemTemplate>
                                        <FooterStyle HorizontalAlign="Center" />
                                        <FooterTemplate>
                                            <asp:Button ID="btAdd" runat="server" Text="+ Agregar" CommandName="AgregarArea" />
                                        </FooterTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="nombre_jefe" HeaderText="Jefe de área" SortExpression="nombre_jefe">
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:BoundField>
                                    <asp:ButtonField ButtonType="Image" CommandName="GestionarSubareas" HeaderText="Gestionar sub-áreas"
                                        ImageUrl="~/Images/Icon/list.png" Text="Gestionar sub-áreas">
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:ButtonField>
                                    <asp:ButtonField HeaderText="Editar" ButtonType="Image" ImageUrl="~/Images/Icon/edit.png"
                                        ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle" Text="Editar"
                                        CommandName="EditarArea">
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:ButtonField>
                                    <asp:ButtonField ButtonType="Image" HeaderText="Eliminar" ImageUrl="~/Images/Icon/remove_file.png"
                                        Text="Eliminar" CommandName="EliminarArea">
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:ButtonField>
                                </Columns>
                                <HeaderStyle BackColor="#666666" ForeColor="White" />
                                <PagerSettings FirstPageText="" LastPageText="" Mode="NextPreviousFirstLast" NextPageText=""
                                    PreviousPageText="" FirstPageImageUrl="~/Images/Icon/first.png" LastPageImageUrl="~/Images/Icon/last.png"
                                    NextPageImageUrl="~/Images/Icon/next.png" PreviousPageImageUrl="~/Images/Icon/previous.png" />
                                <PagerStyle HorizontalAlign="Center" />
                                <RowStyle CssClass="rowData" />
                                <EmptyDataRowStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                <EmptyDataTemplate>
                                    <asp:Image ID="imgEmptyList" runat="server" ImageUrl="~/Images/empty.png" ImageAlign="Middle" />
                                    <asp:Label ID="lbEmptyList" runat="server" Text="  No se han encontrado registros"
                                        Font-Size="14px"></asp:Label>
                                    <br />
                                    <asp:Button ID="btAdd" runat="server" Text="+ Agregar" CommandName="AgregarArea" />
                                </EmptyDataTemplate>
                            </asp:GridView>
                            <asp:SqlDataSource ID="SDSAreas" runat="server" ConnectionString="<%$ ConnectionStrings:NCCSANConnectionString %>"
                                SelectCommand="SELECT ca.nombre_area, p.nombre AS nombre_jefe FROM CentroArea ca, Persona p WHERE (ca.id_centro=@id_centro) AND (p.rut=ca.rut_jefe)">
                                <SelectParameters>
                                    <asp:SessionParameter Name="id_centro" SessionField="id_centro" />
                                </SelectParameters>
                            </asp:SqlDataSource>
                        </td>
                    </tr>
                </table>
                <asp:HiddenField ID="hfAgregarArea" runat="server" />
                <asp:ModalPopupExtender ID="hfAgregarArea_ModalPopupExtender" runat="server" DynamicServicePath=""
                    Enabled="True" TargetControlID="hfAgregarArea" PopupControlID="pnAgregarArea"
                    BackgroundCssClass="Popup_background">
                </asp:ModalPopupExtender>
                <asp:UpdatePanel ID="upAgregarArea" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Panel ID="pnAgregarArea" runat="server" CssClass="Popup_frontground" HorizontalAlign="Center">
                            <table class="tableControls" align="center">
                                <tr>
                                    <td colspan="2" class="tdTitle">
                                        <h1>
                                            Agregar Área
                                        </h1>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="tdUnique">
                                        <asp:Label ID="Label5" runat="server" Text="Nombre"></asp:Label>
                                        &nbsp;<asp:Label ID="Label7" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                                    </td>
                                    <td class="tdUnique">
                                        <asp:TextBox ID="txtNombreArea" runat="server"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="tdUnique">
                                        <asp:Label ID="Label6" runat="server" Text="Jefe de Área"></asp:Label>
                                        &nbsp;<asp:Label ID="Label8" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                                    </td>
                                    <td class="tdUnique">
                                        <asp:TextBox ID="txtNombreJefeArea" runat="server" ReadOnly="True" Width="300px"
                                            Enabled="False"></asp:TextBox>
                                        <asp:Button ID="btBuscarJefeArea" runat="server" Text="Seleccionar" OnClick="btBuscarJefeArea_Click" />
                                        <asp:HiddenField ID="hfRutJefeArea" runat="server" />
                                    </td>
                                </tr>
                            </table>
                            <br />
                            <br />
                            <table align="center">
                                <tr>
                                    <td align="center">
                                        <asp:Button ID="btRegistrarArea" runat="server" Text="Registrar" OnClick="btRegistrarArea_Click"
                                            CssClass="submitButton" />
                                        <asp:Button ID="btCancelarArea" runat="server" Text="Cancelar" OnClick="btCancelarArea_Click"
                                            CssClass="submitButton" />
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <!-- Panel Buscar Persona -->
                <asp:HiddenField ID="hfBuscarPersona" runat="server" />
                <asp:ModalPopupExtender ID="hfBuscarPersona_ModalPopupExtender" runat="server" BackgroundCssClass="Popup_background"
                    DynamicServicePath="" Enabled="True" PopupControlID="upBuscarPersona" TargetControlID="hfBuscarPersona">
                </asp:ModalPopupExtender>
                <asp:UpdatePanel ID="upBuscarPersona" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Panel ID="pnBuscarPersona" runat="server" CssClass="Popup_frontground">
                            <table class="tableControls">
                                <tr>
                                    <td colspan="2" class="tdTitle">
                                        <h1>
                                            Buscar persona</h1>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="tdUnique">
                                        <asp:Label ID="Label22" runat="server" Text="Apellido"></asp:Label>
                                        <asp:TextBox ID="txtBuscarPersonaApellido" runat="server" AutoPostBack="True"></asp:TextBox>
                                        <asp:Button ID="btBuscarPersonaBuscar" runat="server" Text="Buscar" />
                                        <asp:Button ID="btBuscarPersonaLimpiar" runat="server" Text="Limpiar" OnClick="btBuscarPersonaLimpiar_Click" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="tdUnique">
                                        <asp:GridView ID="gvBuscarPersonas" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                                            DataKeyNames="rut" DataSourceID="SDSBuscarPersonas" HorizontalAlign="Center"
                                            OnRowCommand="gvBuscarPersonas_RowCommand" Width="1010px">
                                            <AlternatingRowStyle BackColor="#EDEDED" />
                                            <Columns>
                                                <asp:ButtonField HeaderText="Seleccionar" ButtonType="Image" ImageUrl="~/Images/Icon/add_user.png"
                                                    CommandName="SetJefeArea" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:ButtonField>
                                                <asp:BoundField DataField="rut" HeaderText="RUT" ReadOnly="True" SortExpression="rut" />
                                                <asp:BoundField DataField="nombre" HeaderText="Nombre" SortExpression="nombre" />
                                                <asp:BoundField DataField="edad" HeaderText="Edad" ReadOnly="True" SortExpression="edad">
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="nombre_centro" HeaderText="Centro" 
                                                    SortExpression="nombre_centro">
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="cargo" HeaderText="Cargo" SortExpression="cargo">
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="antiguedad" HeaderText="Antigüedad" ReadOnly="True" SortExpression="antiguedad">
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:BoundField>
                                            </Columns>
                                            <HeaderStyle BackColor="#666666" ForeColor="White" />
                                            <PagerSettings FirstPageText="" LastPageText="" Mode="NextPreviousFirstLast" NextPageText=""
                                                PreviousPageText="" FirstPageImageUrl="~/Images/Icon/first.png" LastPageImageUrl="~/Images/Icon/last.png"
                                                NextPageImageUrl="~/Images/Icon/next.png" PreviousPageImageUrl="~/Images/Icon/previous.png" />
                                            <PagerStyle HorizontalAlign="Center" />
                                        </asp:GridView>
                                        <asp:SqlDataSource ID="SDSBuscarPersonas" runat="server" ConnectionString="<%$ ConnectionStrings:NCCSANConnectionString %>"
                                            SelectCommand="SELECT p.rut, p.nombre, DATEDIFF(YEAR, p.fecha_nacimiento, SYSDATETIME()) AS edad, c.nombre AS nombre_centro, p.cargo, DATEDIFF(YEAR, p.fecha_ingreso, SYSDATETIME()) AS antiguedad FROM Persona p, Centro c WHERE (c.id=p.id_centro) AND (p.nombre LIKE '%' + @nombre + '%') AND (p.fecha_retiro IS NULL) AND (p.nombre_clasificacionpersona&lt;&gt;'Clientes') AND (p.nombre_clasificacionpersona&lt;&gt;'RSP') ORDER BY p.nombre ASC"
                                            OnSelected="SDSBuscarPersonas_Selected">
                                            <SelectParameters>
                                                <asp:ControlParameter ControlID="txtBuscarPersonaApellido" Name="nombre" PropertyName="Text" />
                                            </SelectParameters>
                                        </asp:SqlDataSource>
                                        <div align="left" style="text-align: center">
                                            <asp:Literal ID="ltBuscarPersonaSummary" runat="server"></asp:Literal>
                                        </div>
                                        <br />
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center">
                                        <asp:Button ID="btBuscarPersonaCancelar" runat="server" Text="Cancelar" OnClick="btBuscarPersonaCancelar_Click"
                                            CssClass="submitButton" />
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <!-- Panel Buscar Persona !-->
                <!-- Panel Confirm Eliminar Area -->
                <asp:HiddenField ID="hfEliminarArea" runat="server" />
                <asp:ModalPopupExtender ID="hfEliminarArea_ModalPopupExtender" runat="server" BackgroundCssClass="Popup_background"
                    DynamicServicePath="" Enabled="True" PopupControlID="upEliminarArea" TargetControlID="hfEliminarArea">
                </asp:ModalPopupExtender>
                <asp:UpdatePanel ID="upEliminarArea" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Panel ID="pnEliminarArea" runat="server" CssClass="Popup_frontground">
                            <table align="center">
                                <tr>
                                    <td>
                                        <asp:Label ID="lbMessageEliminarArea" runat="server" CssClass="messageBoxMessage"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                            <br />
                            <br />
                            <table align="center">
                                <tr>
                                    <td align="center">
                                        <asp:Button ID="btEliminarArea" runat="server" Text="Aceptar" CssClass="submitButton"
                                            OnClick="btEliminarArea_Click" />
                                        <asp:Button ID="btCancelarEliminarArea" runat="server" Text="Cancelar" CssClass="submitButton"
                                            OnClick="btCancelarEliminarArea_Click" />
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <!-- Panel Confirm Eliminar Area !-->
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
