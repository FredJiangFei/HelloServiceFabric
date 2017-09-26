import {Component, Input, OnInit, OnChanges} from '@angular/core';

declare var CanvasJS: any;

@Component({
  selector: 'asw-chart',
  template: `<div id="chartContainer" style="height: 400px; width: 100%;"></div>`
})

export class AswChartComponent implements OnInit, OnChanges {
  @Input() public chartItems: any[];

  ngOnInit(): any {
    this.chart();
  }

  ngOnChanges(changes: any) {
    this.chart();
  }

  chart() {
    if (!this.chartItems) {
      return;
    }

    const chartItems = [];
    this.chartItems.forEach(function (c) {
      chartItems.push({
        label: c.page,
        y: parseFloat(c.period)
      });
    });

    const chart = new CanvasJS.Chart('chartContainer', {
      title: {
        text: 'Period per page'
      },
      toolTip: {
        contentFormatter: function (e) {
          let content = '';
          for (let i = 0; i < e.entries.length; i++) {
            content  = calculateSecond(e.entries[i].dataPoint.y);
          }
          return content;
        }
      },

      axisY: {
        labelFormatter: function (e) {
          return calculateSecond(e.value);
        },
      },
      data: [
        {
          type: 'spline',
          dataPoints: chartItems
        }
      ]
    });

    function calculateSecond( s: number) {
      const hours = parseInt(s / 3600 + '');
      const hourLeft = s % 3600;

      const mins = parseInt(hourLeft / 60 + '');
      const seconds = parseFloat((hourLeft % 60).toFixed(1) + '');

      let result = seconds + 's';

      if (mins !== 0) {
        result = mins + 'm' + result;
      }

      if (hours !== 0) {
        result = hours + 'h' + result;
      }

      return result;
    }

    chart.render();
  }
}

