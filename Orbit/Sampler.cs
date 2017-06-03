using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbit
{
    /// <summary>
    /// This class is meant for getting the average value of multiple values stored over time
    /// </summary>
    class Sampler<T>
        where T :
            struct,
            IComparable,
            IComparable<T>,
            IConvertible,
            IEquatable<T>,
            IFormattable
    {
        public delegate void SamplerOverflowHandler(object sender, SamplerEventArgs<T> e);
        public event SamplerOverflowHandler OnSamplerOverflow;

        private int m_sampleCount;
        private T[] m_samples;
        private int m_index;

        public Sampler(int sampleCount)
        {
            if (sampleCount == 0)
            {
                throw new ArgumentException("Sample count must be a positive integer value!");
            }

            m_sampleCount = sampleCount;
            m_samples = new T[sampleCount];
            m_index = 0;
        }

        public void Push(T value)
        {
            m_samples[m_index++] = value;

            // Index overflow
            if (m_index == m_sampleCount)
            {
                // Reset the index
                m_index = 0;

                // Raise the overflow event
                if (OnSamplerOverflow != null)
                {
                    dynamic sum = default(T);

                    foreach (T sample in m_samples)
                    {
                        sum += sample;
                    }

                    SamplerEventArgs<T> args = new SamplerEventArgs<T>(sum / m_sampleCount);
                    OnSamplerOverflow(this, args);
                }
            }
        }
    }

    class SamplerEventArgs<T> : EventArgs
    {
        public T AverageValue { get; private set; }

        public SamplerEventArgs(T averageValue)
        {
            AverageValue = averageValue;
        }
    }
}
