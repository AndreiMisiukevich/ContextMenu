using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ContextMenu
{
	public enum GalleyScrollDirection { Close, Open }
	public enum GalleyScrollState { Closed, Opened, Moving }

	public class ContextMenuScrollView : ScrollView
	{
		public event Action TouchStarted;

		private static ContextMenuScrollView _openedSideMenu;

		private View _contentView;
		private View _contextView;

		protected readonly StackLayout _viewStack;
		protected GalleyScrollDirection _currentDirection;
		protected GalleyScrollState _currentState;
		protected bool _hasBeenAccelerated;
		protected double _prevScrollX;

		protected bool IsOpenDirection => _currentDirection == GalleyScrollDirection.Open;
		protected bool IsDirectionAndStateSame => (int)_currentDirection == (int)_currentState;
		protected virtual bool IsScrollEnabled => true;

		public ContextMenuScrollView()
		{
			Orientation = ScrollOrientation.Horizontal;
			VerticalOptions = LayoutOptions.Fill;
			HorizontalOptions = LayoutOptions.Fill;

			Content = _viewStack = new StackLayout
			{
				VerticalOptions = LayoutOptions.Fill,
				HorizontalOptions = LayoutOptions.Fill,
				Spacing = 0,
				Orientation = StackOrientation.Horizontal
			};

			Scrolled += OnScrolled;
		}

		public View ContentView
		{
			get => _contentView;
			set
			{
				if (value != _contentView)
				{
					if (_contentView != null)
					{
						_viewStack.Children.Remove(_contentView);
					}
					if (value != null)
					{
						_viewStack.Children.Add(value);
					}
				}
				_contentView = value;
			}
		}

		public View ContextView
		{
			get => _contextView;
			set
			{
				if (value != _contextView)
				{
					_viewStack.BatchBegin();
					if (_contextView != null)
					{
						_viewStack.Children.Remove(_contextView);
					}
					if (value != null)
					{
						_viewStack.Children.Add(value);
					}

					if (ContentView.Width < ContentView.WidthRequest)
					{
						ContentView.Layout(new Rectangle(ContentView.X, ContentView.Y, ContentView.WidthRequest, ContentView.Height));
					}
					_viewStack.BatchCommit();
				}
				_contextView = value;
			}
		}

		public bool IsClosed => _currentState == GalleyScrollState.Closed;

		public bool IsOneMenuCanBeOpened { get; set; } = true;

		public virtual void OnTouchStarted()
		{
			TouchStarted?.Invoke();
			_hasBeenAccelerated = false;
		}

		public virtual async void OnTouchEnded()
		{
			if (IsScrollEnabled)
			{
				if (Device.RuntimePlatform == Device.Android)
				{
					await Task.Delay(10);
				}

				if (_hasBeenAccelerated || !IsScrollEnabled)
				{
					return;
				}

				var width = GetContextViewWidth();
				var isOpen = IsOpenDirection
							? ScrollX > GetMovingWidth(width)
							: ScrollX > width - GetMovingWidth(width);
				await ScrollToAsync(isOpen ? width : 0, 0, true);
			}

			if (!_hasBeenAccelerated && IsScrollEnabled && CheckIsOpen() && IsOneMenuCanBeOpened)
			{
				ForceCloseContextMenu(this);
			}
		}

		public virtual async Task OnFlingStarted(bool needScroll = true, bool animated = true, bool inMainThread = false)
		{
			if (needScroll && IsScrollEnabled && IsOneMenuCanBeOpened)
			{
				ForceCloseContextMenu(this);
			}

			_hasBeenAccelerated = true;
			if (needScroll && IsScrollEnabled)
			{

				var task = ScrollToAsync(IsOpenDirection ? GetContextViewWidth() : 0, 0, animated);
				if (inMainThread)
				{
					var completionSource = new TaskCompletionSource<bool>();
					Device.BeginInvokeOnMainThread(async () =>
					{
						await task;
						completionSource.SetResult(true);
					});
					await completionSource.Task;
					return;
				}
				await task;
			}
		}

		public async Task MoveSideMenu(bool isOpen = false, bool animated = true)
		{
			_currentDirection = isOpen
				? GalleyScrollDirection.Open
				: GalleyScrollDirection.Close;

			await OnFlingStarted(!IsDirectionAndStateSame, animated, true);
		}

		protected async void OnMoveActionInvoked(bool isOpen = false)
		{
			await MoveSideMenu(isOpen, true);
		}

		protected virtual void OnScrolled(object sender, ScrolledEventArgs args)
		{
			_currentDirection = Math.Abs(_prevScrollX - ScrollX) < double.Epsilon
					? _currentDirection
					: _prevScrollX > ScrollX
						? GalleyScrollDirection.Close
						: GalleyScrollDirection.Open;
			_prevScrollX = ScrollX;

			if (IsScrollEnabled)
			{
				CheckScrollState();
			}
		}

		protected virtual void CheckScrollState()
		{
			if (Math.Abs(ScrollX) <= double.Epsilon)
			{
				_currentState = GalleyScrollState.Closed;
			}
			else if (Math.Abs(ScrollX - GetContextViewWidth()) <= double.Epsilon)
			{
				_currentState = GalleyScrollState.Opened;
			}
			else
			{
				_currentState = GalleyScrollState.Moving;
			}
		}

		protected virtual double GetContextViewWidth() 
		=> ContentView.Width;

		protected virtual double GetMovingWidth(double contextVidth)
		=> contextVidth * 0.3;

		private bool CheckIsOpen()
		{
			var width = GetContextViewWidth();
			return IsOpenDirection
							? ScrollX > GetMovingWidth(width)
							: ScrollX > width - GetMovingWidth(width);
		}

		public static async void ForceCloseContextMenu(ContextMenuScrollView view)
		{
			if ((_openedSideMenu ?? view) == null || _openedSideMenu == view)
			{
				return;
			}

			try
			{
				var closingSideMenu = _openedSideMenu;
				_openedSideMenu = view;
				if (closingSideMenu?.ScrollX > 0)
				{
					var task = closingSideMenu.ScrollToAsync(0, 0, true);
					if (view == null)
					{
						var completionSource = new TaskCompletionSource<bool>();
						Device.BeginInvokeOnMainThread(async () =>
						{
							await task;
							completionSource.SetResult(true);
						});
						await completionSource.Task;
						return;
					}
					await task;
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}
	}
}
