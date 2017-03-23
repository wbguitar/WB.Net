using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace WB.Commons.Helpers
{
    public static class SortExtensions
    {
        public static void BubbleSort<T>(this IList<T> list)
        {
            list.BubbleSort(Comparer<T>.Default);
        }

        public delegate void bBubbleSortAction(int totalCount, int curCount);
        public delegate void bQuickSortAction(int totalCount, int curCount);

        public static void BubbleSort<T>(this IList<T> list, IComparer<T> comparer, bBubbleSortAction act = null)
        {
            bool stillGoing = true;
            int counter = 0;
            while (stillGoing)
            {
                if (act != null)
                    act(list.Count, counter++);

                stillGoing = false;

                for (int i = 0; i < list.Count - 1; i++)
                {
                    T x = list[i];
                    T y = list[i + 1];
                    if (comparer.Compare(x, y) > 0)
                    {
                        list[i] = y;
                        list[i + 1] = x;
                        stillGoing = true;
                    }
                }
            }
        }

        public static void Quicksort(this List<IComparable> elements, bBubbleSortAction act = null)
        {
            Quicksort(elements, 0, elements.Count, act);
        }

        public static void Quicksort(this List<IComparable> elements, int left, int right, bBubbleSortAction act = null)
        {
            int i = left, j = right;
            var pivot = elements[(left + right) / 2];

            while (i <= j)
            {
                while (elements[i].CompareTo(pivot) < 0)
                {
                    i++;
                }

                while (elements[j].CompareTo(pivot) > 0)
                {
                    j--;
                }

                if (i <= j)
                {
                    // Swap
                    IComparable tmp = elements[i];
                    elements[i] = elements[j];
                    elements[j] = tmp;

                    i++;
                    j--;
                }
            }

            // Recursive calls
            if (left < j)
            {
                Quicksort(elements, left, j);
            }

            if (i < right)
            {
                Quicksort(elements, i, right);
            }
        }


        public static void Quicksort<T>(this List<T> elements, IComparer<T> comparer,
                                        bQuickSortAction act = null)
        {
            if (elements.Count > 0)
                Quicksort(elements, 0, elements.Count - 1, comparer, act);
        }

        public static void Quicksort<T>(this List<T> elements, int left, int right, IComparer<T> comparer, bQuickSortAction act = null)
        {
            #region old
            //int i = left, j = right;
            //var pivot = elements[(left + right) / 2];
            //if (act != null)
            //    act(i, j);

            //while (i <= j)
            //{
            //    while (comparer.Compare(elements[i], pivot) < 0)
            //    {
            //        i++;
            //    }

            //    while (comparer.Compare(elements[j], pivot) > 0)
            //    {
            //        j--;
            //    }

            //    if (i <= j)
            //    {
            //        // Swap
            //        var tmp = elements[i];
            //        elements[i] = elements[j];
            //        elements[j] = tmp;

            //        i++;
            //        j--;
            //    }
            //}

            //// Recursive calls
            //if (left < j)
            //{
            //    Quicksort(elements, left, j, comparer, act);
            //}

            //if (i < right)
            //{
            //    Quicksort(elements, i, right, comparer, act);
            //} 
            #endregion

            var sorter = new QuickSort<T>(elements, comparer);
            sorter.Sort();
        }


        class QuickSort<T>
        {
            List<T> input;
            private IComparer<T> comparer = Comparer<T>.Default;

            public QuickSort(List<T> values, IComparer<T> _comparer = null)
            {
                //input = new T[values.Count];
                //for (int i = 0; i < values.Count; i++)
                //{
                //    input[i] = values[i];
                //}
                input = values;

                if (_comparer != null)
                    comparer = _comparer;
            }

            public List<T> Output
            {
                get
                {
                    return input;
                }
            }

            public void Sort()
            {
                Sorting(0, input.Count - 1);
            }

            private int getPivotPoint(int begPoint, int endPoint)
            {
                int pivot = begPoint;
                int m = begPoint + 1;
                int n = endPoint;
                while ((m < endPoint) &&
                    //(input[pivot].CompareTo(input[m]) >= 0))
                       (comparer.Compare(input[pivot], input[m]) >= 0))
                {
                    m++;
                }

                while ((n > begPoint) &&
                       (comparer.Compare(input[pivot], input[n]) <= 0))
                {
                    n--;
                }
                while (m < n)
                {
                    T temp = input[m];
                    input[m] = input[n];
                    input[n] = temp;

                    while ((m < endPoint) &&
                           (comparer.Compare(input[pivot], input[m]) >= 0))
                    {
                        m++;
                    }

                    while ((n > begPoint) &&
                           (comparer.Compare(input[pivot], input[n]) <= 0))
                    {
                        n--;
                    }

                }
                if (pivot != n)
                {
                    T temp2 = input[n];
                    input[n] = input[pivot];
                    input[pivot] = temp2;

                }
                return n;

            }

            private void Sorting(int beg, int end)
            {
                if (end == beg)
                {
                    return;
                }
                else
                {
                    int pivot = getPivotPoint(beg, end);
                    if (pivot > beg)
                        Sorting(beg, pivot - 1);
                    if (pivot < end)
                        Sorting(pivot + 1, end);
                }
            }
        }
    }
}
