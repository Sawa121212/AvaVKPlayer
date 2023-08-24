using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Threading;
using Common.Core.ToDo;
using Prism.Commands;
using ReactiveUI;

namespace VkPlayer.Domain.Base
{
    public abstract class DataViewModelBase<T> : DataViewModelBase
    {
        public DataViewModelBase()
        {
            AllDataCollection = new ObservableCollection<T>();
            DataCollection = new ObservableCollection<T>();

            ClickCommand = new DelegateCommand<T>(OnSelected);
        }

        /// <summary>
        /// Начать загрузку данных
        /// </summary>
        public virtual void StartLoad() =>
            InvokeHandler.Start(new InvokeHandlerObject(LoadData, this));

        /// <summary>
        /// Загрузить данные
        /// </summary>
        protected virtual void LoadData()
        {
        }

        /// <summary>
        /// Метод, запускаемы при клике
        /// </summary>
        /// <param name="item"></param>
        public abstract void OnSelected(T item);


        /// <summary>
        /// Индекс выбранного элемента
        /// </summary>

        public int SelectedIndex
        {
            get => _selectedIndex;
            set => this.RaiseAndSetIfChanged(ref _selectedIndex, value);
        }

        public virtual void OnSelectedItem()
        {
        }

        /// <summary>
        /// Поиск
        /// </summary>
        /// <param name="text"></param>
        public virtual void Search(string? text)
        {
        }

        /// <summary>
        /// Начать отслеживание поисковой строки
        /// </summary>
        public virtual void StartSearchObservable()
        {
            _searchDisposable ??= this.WhenAnyValue(x => x.SearchText)
                .WhereNotNull()
                .Subscribe(text => Search(text?.ToLower()));
        }

        /// <summary>
        /// Начать отслеживание поисковой строки
        /// </summary>
        public virtual void StartSearchObservable(TimeSpan timeSpan)
        {
            _searchDisposable ??= this.WhenAnyValue(x => x.SearchText)
                .WhereNotNull()
                .Throttle(timeSpan)
                .Subscribe(text => Search(text.ToLower()));
        }

        /// <summary>
        /// Закончить отслеживание поисковой строки
        /// </summary>
        public virtual void StopSearchObservable()
        {
            _searchDisposable?.Dispose();
            _searchDisposable = null;
        }

        public virtual void OnSelectedItem(object sender, PointerPressedEventArgs args)
        {
        }

        /// <summary>
        /// Коллекция элементов
        /// </summary>
        public ObservableCollection<T>? DataCollection
        {
            get => _dataCollection;
            set => this.RaiseAndSetIfChanged(ref _dataCollection, value);
        }

        /// <summary>
        /// Выбранный элементов
        /// </summary>
        public T SelectedItem
        {
            get => _selectedItem;
            set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
        }

        public void StartScrollChangedObservable(Action? action, Orientation orientation)
        {
            _scrolledDisposible =
                this.WhenAnyValue(vm => vm.ScrolledEventArgs)
                    .Subscribe((e) =>
                    {
                        try
                        {
                            double max = 0;
                            double current = 0;

                            if (e?.Source is ScrollViewer scrollViewer)
                            {
                                if (scrollViewer.Name == "LyricsScroll")
                                    return;
                                Dispatcher.UIThread.InvokeAsync(() =>
                                {
                                    if (orientation == Orientation.Vertical)
                                    {
                                        max = scrollViewer.GetValue(ScrollViewer.VerticalScrollBarMaximumProperty);
                                        current = scrollViewer.GetValue(ScrollViewer.VerticalScrollBarValueProperty);
                                    }
                                    else
                                    {
                                        max = scrollViewer.GetValue(ScrollViewer.HorizontalScrollBarMaximumProperty);
                                        current = scrollViewer.GetValue(ScrollViewer.HorizontalScrollBarValueProperty);
                                    }

                                    if (max > 0 && (max == current)) action?.Invoke();
                                });
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                        }
                    });
        }

        public void Scrolled(object sender, ScrollChangedEventArgs args) =>
            ScrolledEventArgs = args;

        public void StopScrollChandegObserVable()
        {
            _scrolledDisposible?.Dispose();
            _scrolledDisposible = null;
        }

        private ScrollChangedEventArgs ScrolledEventArgs
        {
            get => _scrolledEventArgs;
            set => this.RaiseAndSetIfChanged(ref _scrolledEventArgs, value);
        }


        public int ResponseCount
        {
            get => _responseCount;
            set => this.RaiseAndSetIfChanged(ref _responseCount, value);
        }

        public int Offset
        {
            get => _offset;
            set => this.RaiseAndSetIfChanged(ref _offset, value);
        }

        /// <summary>
        /// Флаг видимости поисковой строки
        /// </summary>
        public bool SearchIsVisible
        {
            get => _searchIsVisible;
            set => this.RaiseAndSetIfChanged(ref _searchIsVisible, value);
        }

        /// <summary>
        /// Текст поискового запроса
        /// </summary>
        public string SearchText
        {
            get => _searchText;
            set => this.RaiseAndSetIfChanged(ref _searchText, value);
        }

        /// <summary>
        /// Команда при нажатии на элемент
        /// </summary>
        public ICommand ClickCommand { get; }

        protected ObservableCollection<T>? AllDataCollection;
        private ObservableCollection<T> _dataCollection;
        private IDisposable? _searchDisposable;
        private IDisposable _scrolledDisposible;
        private int _selectedIndex;
        private T _selectedItem;
        private ScrollChangedEventArgs _scrolledEventArgs;
        private int _responseCount;
        private int _offset;
        private bool _searchIsVisible;
        private string _searchText;
    }
}