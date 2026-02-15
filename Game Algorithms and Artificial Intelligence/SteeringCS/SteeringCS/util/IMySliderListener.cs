using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteeringCS
{
    public interface IMySliderListener
    {
         void SliderValueChanged(MySlider source, int value);
    }
}
