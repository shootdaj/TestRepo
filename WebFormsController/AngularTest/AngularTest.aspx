<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AngularTest.aspx.cs" Inherits="WebFormsController.AngularTest" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script src="https://ajax.googleapis.com/ajax/libs/angularjs/1.0.7/angular.min.js"></script>
    <script src="cardsController.js"></script>
    <div ng-app="mainApp">
        <div ng-controller="cardsListController">
            <div id="cardsList">
                <div ng-repeat="card in cards" class="cardItem">
                    Card {{card.id}}
                </div>
            </div>
        </div>
    </div>
</asp:Content>
