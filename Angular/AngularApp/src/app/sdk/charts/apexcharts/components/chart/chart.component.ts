/* -------------------------------------------------------------------------- */
/*                         Enterprise App Builder Toolkit                     */
/*                            Author: Mediasoftpro                            */
/*                       Email: support@mediasoftpro.com                      */
/*       License: Read license.txt located on root of your application.       */
/*                     Copyright 2007 - 2023 @Mediasoftpro                    */
/* -------------------------------------------------------------------------- */
/*
import {
  Component,
  OnInit,
  OnChanges,
  Input,
  SimpleChanges,
  ViewChild,
} from '@angular/core';
import { Store } from '@ngrx/store';
import { IAppState } from '../../../../../store_v2/model.store';
import {
  ChartOptions,
  PieChartOptions,
  ApexChartService,
} from '../../chartService';
import { DataService } from '../../services/DataService';
import * as coreSelectors from '../../../../../store_v2/core/core.selector';
import { CoreService } from '../../../../services/coreService';
import { LoaderComponent } from '../../../../../sdk/components/loader/loader.component';
import { NgApexchartsModule } from 'ng-apexcharts';
import {
  NgIf,
  NgClass,
  JsonPipe,
  NgFor,
  NgSwitch,
  DatePipe,
  NgSwitchCase,
  AsyncPipe,
} from '@angular/common';
@Component({
  selector: 'app-core-chart',
  templateUrl: './chart.html',
  standalone: true,
  imports: [
    NgApexchartsModule,
    NgIf,
    NgSwitch,
    NgSwitchCase,
    NgFor,
    NgClass,
    JsonPipe,
    LoaderComponent,
    DatePipe,
  ],
  providers: [ApexChartService, DataService],
})
export class CoreChartComponent implements OnInit, OnChanges {
  public chartOptions: Partial<ChartOptions> = {};
  public pieChartOptions: Partial<PieChartOptions> = {};

  @Input() Title: string = '';
  @Input() ContainerCss: string = 'card-body';
  @Input() Filters: any = [];
  @Input() showFilter: boolean = true;
  @Input() yAxisTitle: string = 'Analytics';
  @Input() ChartHeight: number = 550;
  @Input() isResponsive: boolean = false;
  @Input() Query: any = {};
  @Input() APIUrl: string = '';
  @Input() ChartType: string = 'column'; // column, bar, area, line, donut, pie
  @Input() Colors: string[] = ['#5d1661', '#179174'];
  @Input() ColumnWidth: string = '5%';
  @Input() SeriesOptions: any[] = [
    {
      title: 'Total',
      prop: 'total',
      data: [],
    },
  ];
  @Input() SeriesData: any[] = [];
  @ViewChild('chart') chart: any;
  showLoader = false;
  RecordExist = false;
  Message = '';
  Stacked: boolean = false;
  readonly custom_event$ = this._store.pipe(select(coreSelectors.event));

  constructor(
    
    private dataService: DataService,
    private chartService: ApexChartService,
    private coreService: CoreService
  ) {
    this.custom_event$.subscribe((filter: any) => {
      if (filter.type === 'datefilter') {
        if (this.Query.order_entity !== null) {
          this.Query.order_entity.datefilter = parseInt(
            filter.data.datefilter,
            10
          );
        } else {
          this.Query.datefilter = parseInt(filter.data.datefilter, 10);
        }
        this.prepareQuery();
      }
    });
  }

  ngOnInit() {
    this.prepareQuery();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if ( this.SeriesData.length > 0 ) {
        this.chart.updateSeries([
          {
            name: 'candle',
            data: this.SeriesData
          }
        ])
         
       }

    //this.prepareQuery();
  }

  filterReport(value: number, event: any) {
    if (this.Query.order_entity !== undefined) {
      this.Query.order_entity.datefilter = value;
    } else {
      this.Query.datefilter = value;
    }

    this.prepareQuery();
  }

  prepareQuery() {
    if (this.SeriesData.length > 0) {
      this.RecordExist = true;
      this.renderCandleStickChart(this.SeriesData);
    } else {
      if (this.APIUrl === null || this.APIUrl === '') {
        this.Message = 'Invalid API Call';
      } else {
        this.getStats();
      }
    }
  }

  getStats() {
    this.showLoader = true;
    this.dataService
      .getData(this.Query, this.APIUrl)
      .pipe()
      .subscribe((data: any) => {
        if (data.posts !== undefined) {
          if (data.posts.length === 0) {
            this.RecordExist = false;
            this.Message = 'No data found!';
          } else {
            this.RecordExist = true;
            this._ProcessData(data.posts);
          }
          this.showLoader = false;
        }
      });
  }

  _resetSeriesOptions() {
    for (let item of this.SeriesOptions) {
      item.data = [];
    }
  }

  _ProcessData(data: any) {
    this._resetSeriesOptions();
    let _label: any = [];
    let Series: any = [];
    for (let item of data) {
      for (let p of this.SeriesOptions) {
        p.data.push(item[p.prop]);
      }
      _label.push(item.label);
    }
    let group_by = this.Query.groupby!;
    let chart_type = this.ChartType;

    if (chart_type !== 'pie' && chart_type !== 'donut') {
      for (let item of this.SeriesOptions) {
        Series.push({
          name: item.title,
          data: item.data,
        });
      }

      this.renderNormalChart(Series, chart_type, _label, group_by);
    } else {
      let _labels: any[] = [];
      let _series: any[] = [];
      for (let item of this.SeriesOptions) {
        _labels.push(item.title);
        let sum_of_data: number = 0;
        for (let d of item.data) {
          sum_of_data = sum_of_data + d;
        }
        _series.push(sum_of_data);
      }

      this.renderPieChart(chart_type, _series, _labels);
    }
  }

  renderCandleStickChart(data: any) {
    this.chartOptions = {
      series: [
        {
          name: 'candle',
          data: data,
        },
      ],
      chart: {
        type: 'candlestick',
        height: 350,
        width: '100%',
      },
      title: {
        text: this.Title,
        align: 'left',
      },
      legend: {
        show: true,
        fontSize: '14px',
        position: 'top',
      },
      dataLabels: {
        enabled: false,
      },
      stroke: {
        curve: 'straight',
      },
      xaxis: {
        type: 'category',
        // type: "datetime"
      },
      plotOptions: {
        bar: {
          columnWidth: 10,
        },
        candlestick: {
          wick: {
            useFillColor: true,
          },
          colors: {
            upward: '#14b53c',
            downward: '#ff0000'
          }
        },
      },
      yaxis: {
        tooltip: {
          enabled: true,
        },
      },
    };
  }

  renderNormalChart(data: any, type: string, labels: any, group_by: number) {
    this.chartService.initializeChart(
      this.chartOptions,
      type,
      '',
      '',
      this.ChartHeight,
      this.Stacked
    );
    this.chartOptions.series = data;
    if (type === 'bar' || type === 'column') {
      let horizontal = true;
      if (type === 'column') {
        horizontal = false;
      }
      this.chartOptions.plotOptions = {
        bar: {
          horizontal: horizontal,
          columnWidth: this.ColumnWidth,
        },
      };
    }
    if (this.Colors.length > 0) {
      this.chartOptions.fill = {
        colors: this.Colors,
      };
    }
   
    if (type === 'bar') {
      this.chartOptions.xaxis = {
        categories: labels,
      };
    } else if (type !== 'bar') {
      if (labels !== '') {
        this.chartOptions.labels = labels;
      }

      this.chartOptions.yaxis = {
        opposite: false,
        labels: {
          show: true,
          formatter: function (val: number) {
            if (typeof val === 'string') {
              return val;
            } else {
              return val.toFixed(0);
            }
          },
        },
        title: {
          text: this.yAxisTitle,
        },
        axisBorder: {
          show: true,
        },
        axisTicks: {
          show: true,
        },
      };
    }
  }

  renderPieChart(type: string, series: any, labels: any) {
    this.chartService.initializePieChart(
      this.pieChartOptions,
      type,
      '',
      this.ChartHeight,
      200,
      this.isResponsive
    );
    this.pieChartOptions.series = series;
    this.pieChartOptions.labels = labels;
  }

  getKey(index: number, item: any): string {
    return item.key;
  }
}
*/