import { Injectable } from '@angular/core';
import {
  ApexAxisChartSeries,
  ApexNonAxisChartSeries,
  ApexResponsive,
  ApexChart,
  ApexXAxis,
  ApexYAxis,
  ApexDataLabels,
  ApexTitleSubtitle,
  ApexStroke,
  ApexGrid,
  ApexFill,
  ApexTheme,
  ApexLegend,
  ApexTooltip,
  ApexPlotOptions,
} from 'ng-apexcharts';

export type ChartOptions = {
  series: ApexAxisChartSeries;
  chart: ApexChart;
  xaxis: ApexXAxis;
  yaxis: ApexYAxis;
  dataLabels: ApexDataLabels;
  colors: string[];
  grid: ApexGrid;
  stroke: ApexStroke;
  title: ApexTitleSubtitle;
  legend: ApexLegend;
  fill: ApexFill;
  tooltip: ApexTooltip;
  labels: any;
  theme: ApexTheme;
  responsive: ApexResponsive[];
  subtitle: ApexTitleSubtitle;
  plotOptions: ApexPlotOptions;
};

export type PieChartOptions = {
  series: ApexNonAxisChartSeries;
  chart: ApexChart;
  responsive: ApexResponsive[];
  labels: any;
  theme: ApexTheme;
  title: ApexTitleSubtitle;
  fill: ApexFill;
  legend: ApexLegend;
  dataLabels: ApexDataLabels;
};

@Injectable()
export class ApexChartService {
  constructor() {}

  // Initialize Line, Area, Column, Bar Charts
  initializeChart(
    options: Partial<ChartOptions>,
    type: any = 'bar',
    title: string,
    sub_title: string,
    height: number,
    stacked: boolean
  ) {
    if (title !== '') {
      options.title = {
        text: title,
        align: 'left',
      };
    }

    if (sub_title !== '') {
      options.subtitle = {
        text: sub_title,
        align: 'left',
      };
    }

    let _type = type;
    if (_type === 'column') {
      _type = 'bar';
    }
    options.chart = {
      height: height,
      type: _type,
      stacked: stacked,
      // stackType: '100%',
      toolbar: {
        show: true,
        tools: {
          download: false,
        },
      },
      zoom: {
        enabled: false,
      },
    };

    /*options.fill = {
        type: "gradient",
        gradient: {
          shadeIntensity: 1,
          inverseColors: false,
          opacityFrom: 0.5,
          opacityTo: 0,
          stops: [0, 90, 100]
        }
    }*/

    // move legend top right section */
    /*options.legend = {
        position: "top",
        horizontalAlign: "right",
        floating: true,
        offsetY: -25,
        offsetX: -5
    }*/

    if (type === 'bar' || type === 'column') {
      options.dataLabels = {
        enabled: false,
        /*offsetX: -6,
          style: {
            fontSize: "12px",
            colors: ["#fff"]
          }*/
      };
    }

    /*
    // display label on top
    options.dataLabels = {
        enabled: true,
        formatter: function(val) {
          return val + "%";
        },
        offsetY: -20,
        style: {
          fontSize: "12px",
          colors: ["#304758"]
        }
      }
    */

    if (type === 'line') {
      options.stroke = {
        curve: 'smooth',
      };
    }

    options.grid = {
      row: {
        colors: ['#f3f3f3', 'transparent'],
        opacity: 0.5,
      },
    };

    /*if (type === 'bar' || type === 'column') {
      let horizontal = true;
      if (type === 'column') {
        horizontal = false;
      }
      options.plotOptions = {
        bar: {
          horizontal: horizontal, // true: bar, false, column chart
          //columnWidth: "55%"
        },
      };
    }*/

    if (type !== 'pie' && type !== 'donut') {
      /*options.yaxis = {
            opposite: false,
            labels: {
              show: true,
              formatter: function(val) {
                return val.toFixed(0);
              }
            },
            title: {
              text: yaxis_title
            },
            axisBorder: {
                show: true
              },
              axisTicks: {
                show: true
              },
        };*/

      options.xaxis = {
        // type: "datetime",
        categories: [
          /*"Jan",
                "Feb",
                "Mar",
                "Apr",
                "May",
                "Jun",
                "Jul",
                "Aug",
                "Sep"*/
        ],
        axisBorder: {
          show: true,
        },
        axisTicks: {
          show: true,
        },
      };
    }

    /*options.tooltip = {
        shared: false,
        x: {
            format: "dd/MM/yy HH:mm"
        },
        y: {
            formatter: function(val) {
            return (val / 1000000).toFixed(0);
            }
        },
        y: {
            formatter: function(val) {
              return "$ " + val + " thousands";
            }
        }
    };*/

    options.labels = [
      /*
        "Monday",
        "Tuesday",
        "Wednesday",
        "Thursday",
        "Friday",
        "Saturday"
        */
    ];

    options.theme = {
      monochrome: {
        enabled: false,
      },
    };

    if (type === 'pie' || type === 'donut') {
      options.responsive = [
        {
          breakpoint: 480,
          options: {
            chart: {
              width: 200,
            },
            legend: {
              position: 'bottom',
            },
          },
        },
      ];
    }
  }

  // Initialize Pie / Donut Charts (type => donut, pie)
  initializePieChart(
    options: Partial<PieChartOptions>,
    type: any = 'pie',
    title: string,
    height: number = 350,
    width: number = 200,
    isresponsive: boolean = false
  ) {
    
    options.chart = {
      height: height,
      type: type,
    };

    if (title !== '') {
      options.title = {
        text: title,
      };
    }

    options.dataLabels = {
      enabled: false, // show, hide labels
    };
    options.theme = {
      monochrome: {
        enabled: true,
      },
    };

    if ( isresponsive ) {
      options.responsive = [
        {
          breakpoint: 480,
          options: {
            chart: {
              width: 200,
            },
            legend: {
              position: 'bottom',
            },
          },
        },
      ];
    }
   
  }
}
