/* -------------------------------------------------------------------------- */
/*                         Enterprise App Builder Toolkit                     */
/*                            Author: Mediasoftpro                            */
/*                       Email: support@mediasoftpro.com                      */
/*       License: Read license.txt located on root of your application.       */
/*                     Copyright 2007 - 2023 @Mediasoftpro                    */
/* -------------------------------------------------------------------------- */

import { Component, OnInit, OnDestroy, Input } from '@angular/core';
import { Router } from '@angular/router';
import { SettingsService } from '../../services/SettingService';
import { DataService } from '../../services/DataService';
import {
  ChartOptions,
  PieChartOptions,
  ApexChartService,
} from '../../chartService';
import { IAnalyticNavigation } from '../../services/Types';
import { LoaderComponent } from '../../../../../sdk/components/loader/loader.component';
import {
  NgIf,
  NgClass,
  JsonPipe,
  NgFor,
  NgSwitch,
  DatePipe,
  NgSwitchCase,
  AsyncPipe,
  Location
} from '@angular/common';
import { DynamicModalFormComponent } from '../../../../../sdk/components/reactiveform/dynamic-modal-form';
//import { NavButtonsComponent } from '../../../../../sdk/components/navbuttons/buttons.component';
import { NgApexchartsModule } from 'ng-apexcharts';
@Component({
  selector: 'app-core-analytic',
  templateUrl: './analytic.html',
  standalone: true,
  imports: [
    NgIf,
    NgSwitch,
    NgSwitchCase,
    NgFor,
    NgClass,
    JsonPipe,
    LoaderComponent,
    DatePipe,
    DynamicModalFormComponent,
    //NavButtonsComponent,
    NgApexchartsModule
  ],
  providers: [ApexChartService, SettingsService, DataService],
})
export class CoreAnalyticComponent implements OnInit, OnDestroy {
  public chartOptions: Partial<ChartOptions> = {};
  public pieChartOptions: Partial<PieChartOptions> = {};

  @Input() Title: string = 'Analytic Report';
  @Input() yAxisTitle: string = 'Analytics';
  // @Input() ReportType: number = 2; // 0: analytic, 1: sale, 2: both sale, analytic
  @Input() SeriesOptions: any[] = [
    {
      title: 'Total Properties',
      prop: 'total',
      data: [],
    }
  ];

  @Input() ActionGranted: Boolean = false;
  @Input() ChartHeight: number = 550;
  @Input() Query: any = {};
  @Input() APIUrl: string = '';
  @Input() ChartType: string = 'column'; // column, bar, area, line, donut, pie
  @Input() ColumnWidth: string = '10%';
  @Input() isResponsive: boolean = false;
  @Input() Stacked: boolean = false;
  @Input() Nav: IAnalyticNavigation = {
    Options: [],
    ButtonText: 'Generate Report',
    Buttons: [],
    ButtonLinks: [],
  };

  constructor(
    private settingService: SettingsService,
    public dataService: DataService,
    private chartService: ApexChartService,
    private router: Router,
    private _location: Location
  ) {}

  RecordExist = true;
  DataLoaded = false;
  FilterOptions: any = {};
  showLoader = false;
  
  ngOnInit() {}

  toolbaraction(selection: any) {
    switch(selection.action) {
       case 'back':
          this._location.back();
         break;
    }
  
  }

  onNavigationDropdownSelection(payload: any) {
    // on dropdown selection
  }

  GenerateReport(filter_options: any) {
    for (let prop in filter_options) {
      this.Query[prop] = filter_options[prop];
    }

    this.ChartType = this.Query.chart_type;

    this.renderReport();
  }

  renderReport() {
    this.showLoader = true;
    this.dataService
      .getData(this.Query, this.APIUrl)
      .pipe()
      .subscribe((data: any) => {
        if (data.posts !== undefined) {
          if (data.posts.length === 0) {
            this.RecordExist = false;
          } else {
            this.RecordExist = true;
            this.DataLoaded = true;

            this._ProcessData(data.posts);
          }
          this.showLoader = false;
        }
      });
  }

  _resetSeriesOptions() {
      for (let item of this.SeriesOptions) {
         item.data = []
      }
  }

  _ProcessData(data: any) {
    this._resetSeriesOptions()
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
      
      let _labels: any[] = []
      let _series: any[] = []
      for (let item of this.SeriesOptions) {
          _labels.push(item.title)
          let sum_of_data: number = 0;
          for (let d of item.data) {
            
             sum_of_data = sum_of_data + d
          }
          
          _series.push(sum_of_data)
      }
     
      this.renderPieChart(chart_type, _series, _labels);
    }
    
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
    /*if (this.Colors.length > 0) {
      this.chartOptions.fill = {
        colors: this.Colors
      }
    }*/
    /*this.chartOptions.tooltip = {
      shared: false,
      x: {
        format: 'dd/MM/yy HH:mm',
      },
    };*/
    if (type === 'bar') {
      this.chartOptions.xaxis = {
        categories: labels,
      };
    } else if (type !== 'bar') {
      this.chartOptions.labels = labels;

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

  ngOnDestroy(): void {}
}
