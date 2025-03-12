using Syncfusion.Maui.DataSource;
using Syncfusion.Maui.DataSource.Extensions;
using Syncfusion.Maui.ListView;

namespace MauiListView
{
    public class Behavior : Behavior<ContentPage>
    {
        private Grid? indexPanelGrid;
        private Label? label;
        private Label? previousLabel;
        public SfListView? ListView { get; private set; }

        protected override void OnAttachedTo(ContentPage bindable)
        {
            ListView = bindable.FindByName<SfListView>("listView");
            indexPanelGrid = bindable.FindByName<Grid>("IndexPanelGrid");
            
            // grouping items based on ContactName
            ListView.DataSource!.GroupDescriptors.Add(new GroupDescriptor()
            {
                PropertyName = "ContactName",
                KeySelector = (object obj1) =>
                {
                    var item = (obj1 as Contacts);
                    return item!.ContactName[0].ToString();
                }
            });
            ListView.Loaded += ListView_Loaded; 
            base.OnAttachedTo(bindable);
        }

        #region Private Methods
        private void ListView_Loaded(object? sender, ListViewLoadedEventArgs e)
        {
            indexPanelGrid!.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));

            //Creating new label based on the group header key value after loading
            var groupcount = this.ListView!.DataSource!.Groups.Count;

            var items = ListView.DataSource.Groups.ToList<GroupResult>().OrderBy(o=>o.Key);           
            for (int i = 0; i < groupcount; i++)
            {
                label = new Label();
                GroupResult group = items.ElementAt(i);
                label.Text = group.Key.ToString();

                indexPanelGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
                Grid.SetColumn(label, 0);
                Grid.SetRow(label, i);
                indexPanelGrid.Children.Add(label);

                var labelTapped = new TapGestureRecognizer() { Command = new Command<object>(OnTapped), CommandParameter = label };
                label.GestureRecognizers.Add(labelTapped);
            }
        }
        private void OnTapped(object obj)
        {
            // Navigating to respective GroupHeader based on the tapped label value loaded in IndexLabelGrid
            if (previousLabel != null)
            {
                previousLabel.TextColor = Colors.DimGray;
            }
            var label = obj as Label;
            var text = label!.Text;
            label.TextColor = Colors.Red;
            for (int i = 0; i < ListView!.DataSource!.Groups.Count; i++)
            {
                var group = ListView.DataSource.Groups[i];

                if (group.Key == null)
                    App.Current!.MainPage!.DisplayAlert("Message", "Group not available", "ok");
                if ((group.Key != null && group.Key.Equals(text)))
                {
                    ListView.ItemsLayout!.ScrollToRowIndex(ListView.DataSource.DisplayItems.IndexOf(group), true);
                }
            }
            previousLabel = label;
        }
        #endregion
    }
}
