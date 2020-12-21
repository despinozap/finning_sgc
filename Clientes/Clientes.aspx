<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Clientes.aspx.cs" Inherits="NCCSAN.Clientes.Clientes" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
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
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="divTituloForm">
        <h1>
            Registros</h1>
    </div>
    <asp:UpdatePanel ID="uPanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
                <table align="center" class="tableControls">
                    <tr>
                        <td class="tdTitle">
                            <h1>
                                Simbología de estados</h1>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <p>
                                En esta tabla se detalla la simbología utilizada en la lista general de Eventos para representar la serie de estados por los que pasa en su ciclo de vida.
                                Para cada estado se muestra el ícono utilizado junto a una breve descripción.
                                <br />
                                Los estados marcados con el ícono rojo de alerta (<asp:Image ID="Image9" 
                                    runat="server" ImageUrl="~/Images/Icon/ball_red_step.gif" 
                                    ImageAlign="AbsMiddle" />) requieren ser tratados a la brevedad.
                            </p>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique" align="center">
                            <asp:Table ID="tbSimbologia" runat="server" Width="100%">
                                <asp:TableHeaderRow>
                                    <asp:TableHeaderCell Text="Ícono" BackColor="#666666" ForeColor="White" Width="10%" />
                                    <asp:TableHeaderCell Text="Estado" BackColor="#666666" ForeColor="White" Width="20%" />
                                    <asp:TableHeaderCell Text="Descripción" BackColor="#666666" ForeColor="White" Width="70%" />
                                </asp:TableHeaderRow>
                                <asp:TableRow>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:Image ID="Image10" runat="server" ImageUrl="~/Images/Icon/ball_alert.gif" />
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:Label ID="Label17" runat="server" Text="Revisión pendiente"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Left">
                                        <asp:Label ID="Label18" runat="server" Text="Representa un Evento que ha sido registrado por el Cliente pero necesita ser revisado por el área de calidad para determinar el IRC y la criticidad"></asp:Label>
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:Image ID="Image1" runat="server" ImageUrl="~/Images/Icon/ball_red_step.gif" />
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:Label ID="Label2" runat="server" Text="Acción inmediata pendiente"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Left">
                                        <asp:Label ID="Label5" runat="server" Text="Representa un Evento que ha sido valorado con un IRC < 10"></asp:Label>
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:Image ID="Image2" runat="server" ImageUrl="~/Images/Icon/ball_red.png" />
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:Label ID="Label1" runat="server" Text="Investigación pendiente"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Left">
                                        <asp:Label ID="Label3" runat="server" Text="Representa un Evento que ha sido valorado con un IRC >= 10 ó que tiene fuente 'Reclamo de Cliente'"></asp:Label>
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:Image ID="Image3" runat="server" ImageUrl="~/Images/Icon/ball_yellow.png" />
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:Label ID="Label4" runat="server" Text="Investigación en curso"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Left">
                                        <asp:Label ID="Label6" runat="server" Text="Representa una Investigación que ha sido iniciada y permanece a la espera de la documentación que debe entregar el responsable al encargado de Calidad"></asp:Label>
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:Image ID="Image4" runat="server" ImageUrl="~/Images/Icon/ball_red_step.gif" />
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:Label ID="Label7" runat="server" Text="Evaluación pendiente"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Left">
                                        <asp:Label ID="Label8" runat="server" Text="Representa una Investigación que concluyó. El responsable entregó los antecedentes y se debe registrar la decisión tomada por el Comité de Calidad (aceptación)"></asp:Label>
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:Image ID="Image5" runat="server" ImageUrl="~/Images/Icon/ball_red.png" />
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:Label ID="Label9" runat="server" Text="Plan de acción pendiente"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Left">
                                        <asp:Label ID="Label10" runat="server" Text="Representa un Evento con IRC >= 10 que ya tiene asociada una Investigación y una Evaluación. Permanece a la espera de ingresar el Plan de Acción determinado por el Comité de Calidad junto a las tareas que lo componen y los responsables de su ejecución"></asp:Label>
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:Image ID="Image6" runat="server" ImageUrl="~/Images/Icon/ball_yellow.png" />
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:Label ID="Label11" runat="server" Text="Plan de acción en curso"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Left">
                                        <asp:Label ID="Label12" runat="server" Text="Representa un Plan de Acción que está en ejecución, es decir, la totalidad de sus tareas no se han ejecutado o el encargado de Calidad no ha revisado para registrar el cierre"></asp:Label>
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:Image ID="Image7" runat="server" ImageUrl="~/Images/Icon/ball_grey.png" />
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:Label ID="Label13" runat="server" Text="Verificación pendiente"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Left">
                                        <asp:Label ID="Label14" runat="server" Text="Representa un Plan de Acción que concluyó y el encargado de Calidad ya cerró. Permanece a la espera de la Verificación (a largo plazo) para comprobar que realmente evitó la ocurrencia de incidentes de similares condiciones"></asp:Label>
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:Image ID="Image8" runat="server" ImageUrl="~/Images/Icon/ball_green.png" />
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Center">
                                        <asp:Label ID="Label15" runat="server" Text="Cerrado"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Left">
                                        <asp:Label ID="Label16" runat="server" Text="Representa un Evento que ha terminado el ciclo de vida. El Evento puede llegar al estado 'Cerrado' debido a una Acción Inmediata registrada, Evaluación rechazada o Plan de Acción verificado"></asp:Label>
                                    </asp:TableCell>
                                </asp:TableRow>
                            </asp:Table>
                        </td>
                    </tr>
                </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
