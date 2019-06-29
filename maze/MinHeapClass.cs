using System;
using System.Collections.Generic;
using System.Text;

namespace maze
{
    
    class Maze_element
    {
        public int x;
        public int y;
        public int f;
        public int g;
        public int h;
        public int heapIndex;
        public Maze_element fromNode;


        public Maze_element(int i, int j)
        {
            x = i;
            y = j;
            f = Int32.MaxValue;
            g = Int32.MaxValue;
            h = 0;
            heapIndex = -1;
            fromNode = null;
        }
    }
    class MinHeapClass
    {
        private readonly Maze_element[] _elements;
        public int _size = 0;

        public MinHeapClass(int size)
        {
            _elements = new Maze_element[size];
        }

        private int GetLeftChildIndex(int elementIndex) => 2 * elementIndex + 1;
        private int GetRightChildIndex(int elementIndex) => 2 * elementIndex + 2;
        private int GetParentIndex(int elementIndex) => (elementIndex - 1) / 2;

        private bool HasLeftChild(int elementIndex) => GetLeftChildIndex(elementIndex) < _size;
        private bool HasRightChild(int elementIndex) => GetRightChildIndex(elementIndex) < _size;
        private bool IsRoot(int elementIndex) => elementIndex == 0;

        private Maze_element GetLeftChild(int elementIndex) => _elements[GetLeftChildIndex(elementIndex)];
        private Maze_element GetRightChild(int elementIndex) => _elements[GetRightChildIndex(elementIndex)];
        private Maze_element GetParent(int elementIndex) => _elements[GetParentIndex(elementIndex)];

        private void Swap(int firstIndex, int secondIndex)
        {
            var temp = _elements[firstIndex];
            _elements[firstIndex] = _elements[secondIndex];
            _elements[secondIndex] = temp;

            _elements[firstIndex].heapIndex = firstIndex;
            _elements[secondIndex].heapIndex = secondIndex;
        }

        public bool IsEmpty()
        {
            return _size == 0;
        }

        public Maze_element Peek()
        {
            if (_size == 0)
                throw new IndexOutOfRangeException();

            return _elements[0];
        }

        public Maze_element Pop()
        {
            if (_size == 0)
                throw new IndexOutOfRangeException();

            var result = _elements[0];
            _elements[0] = _elements[_size - 1];
            _size--;

            ReCalculateDown(0);

            return result;
        }

        public void Add(Maze_element element)
        {
            if (_size == _elements.Length)
                throw new IndexOutOfRangeException();

            _elements[_size] = element;
            element.heapIndex = _size;
            _size++;

            ReCalculateUp(_size - 1);
        }

        public void ReCalculateDown(int k)
        {
            int index = k;
            while (HasLeftChild(index))
            {
                var smallerIndex = GetLeftChildIndex(index);
                if (HasRightChild(index) && GetRightChild(index).f < GetLeftChild(index).f)
                {
                    smallerIndex = GetRightChildIndex(index);
                }

                if (_elements[smallerIndex].f >= _elements[index].f)
                {
                    break;
                }

                Swap(smallerIndex, index);
                index = smallerIndex;
            }
        }

        public void ReCalculateUp(int k)
        {
            var index = k;
            while (!IsRoot(index) && _elements[index].f < GetParent(index).f)
            {
                var parentIndex = GetParentIndex(index);
                Swap(parentIndex, index);
                index = parentIndex;
            }
        }
    }
}
