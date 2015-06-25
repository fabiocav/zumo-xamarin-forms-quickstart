﻿using System;
using System.Collections.Generic;

using Xamarin.Forms;
using System.Threading.Tasks;

namespace fcxfquickstart
{
	public partial class TodoList : ContentPage
	{
		TodoItemManager manager;

		public TodoList ()
		{
			InitializeComponent ();

			manager = new TodoItemManager ();
		}

		protected override async void OnAppearing ()
		{
			base.OnAppearing ();

			todoList.ItemsSource = await manager.GetTodoItemsAsync ();
		}

		// Data methods

		async Task AddItem (TodoItem item) {
			await manager.SaveTaskAsync(item);
			todoList.ItemsSource = await manager.GetTodoItemsAsync ();
		}
		async Task DeleteItem (TodoItem item) {
			await manager.DeleteTaskAsync(item);
			todoList.ItemsSource = await manager.GetTodoItemsAsync ();
		}

		public async void OnAdd (object sender, EventArgs e) {
			var todo = new TodoItem { Name = newItemName.Text };
			await AddItem (todo);
			newItemName.Text = "";
			newItemName.Unfocus ();
		}

		// Event handlers

		public async void OnSelected (object sender, SelectedItemChangedEventArgs e) {
			var todo = e.SelectedItem as TodoItem;
			if (Device.OS != TargetPlatform.iOS && todo != null) {
				// Not iOS - the swipe-to-delete is discoverable there
				if (Device.OS == TargetPlatform.Android) {
					await DisplayAlert (todo.Name, "Press-and-hold to delete task " + todo.Name, "Got it!");
				} else {
					// Windows, not all platforms support the Context Actions yet
					if (await DisplayAlert ("Delete?", "Do you wish to delete " + todo.Name + "?", "Delete", "Cancel")) {
						await DeleteItem (todo);
					}
				}
			}
			// prevents background getting highlighted
			todoList.SelectedItem = null;
		}

		// http://developer.xamarin.com/guides/cross-platform/xamarin-forms/working-with/listview/#context
		public async void OnDelete (object sender, EventArgs e) {
			var mi = ((MenuItem)sender);
			var todo = mi.CommandParameter as TodoItem;
			await DeleteItem (todo);
		}

		// http://developer.xamarin.com/guides/cross-platform/xamarin-forms/working-with/listview/#pulltorefresh
		public async void OnRefresh (object sender, EventArgs e) {
			var list = (ListView)sender;
            var success = false;
			try {
				todoList.ItemsSource = await manager.GetTodoItemsAsync ();
                success = true;
			} catch (Exception ex) {
                // requires C# 6
				//await DisplayAlert ("Refresh Error", "Couldn't refresh data ("+ex.Message+")", "OK");
			}
			list.EndRefresh();
            if (!success)
                await DisplayAlert("Refresh Error", "Couldn't refresh data", "OK");
		}
	}
}

