import { Component, OnInit, ViewChild, ElementRef, AfterViewInit, OnDestroy, Input, OnChanges, SimpleChanges } from '@angular/core';
import Chart, { ChartConfiguration, ChartData, ChartOptions } from 'chart.js/auto';

@Component({
  selector: 'app-doughnut-chart',
  templateUrl: './doughnut-chart.component.html',
  styleUrls: ['./doughnut-chart.component.scss']
})
export class DoughnutChartComponent implements OnInit, AfterViewInit, OnDestroy, OnChanges {
  @ViewChild('myDoughnutChart') chartCanvas!: ElementRef;
  private chart: Chart | undefined;

  @Input() title: string = 'Doughnut Sample';
  @Input() subtitle: string = '';
  @Input() chartData: ChartData<'doughnut'> = {
    labels: [],
    datasets: []
  };

  // Define constant chart options within the component
  // These options will not be externally configurable via @Input()
  private readonly constantChartOptions: ChartOptions<'doughnut'> = {
    responsive: true,
    maintainAspectRatio: false, // Allows the chart to resize based on its container
    plugins: {
      legend: {
        position: 'top', // Position the legend at the top
        labels: {
          font: {
            size: 14 // Example: make legend labels slightly larger
          }
        }
      },
      title: {
        display: true,
        text: this.subtitle,
        font: {
          size: 18, // Example: larger title font
          weight: 'bold'
        },
        color: '#333' // Example: title color
      }
    },
    // You can add more global options here that apply to all instances
    cutout: '70%', // Makes the hole larger, common for doughnut charts
    animation: {
      animateRotate: true,
      animateScale: false
    }
  };

  constructor() { }

  ngOnInit(): void {
    // Initialization logic if needed
  }

  ngAfterViewInit(): void {
    this.createDoughnutChart();
  }

  /**
   * ngOnChanges is called when any data-bound input property of a directive or component changes.
   * We now only listen for changes in chartData.
   */
  ngOnChanges(changes: SimpleChanges): void {
    // Check if chartData has changed and if the chart already exists
    if (changes['chartData'] && this.chart) {
      this.updateDoughnutChart();
    }
  }

  ngOnDestroy(): void {
    if (this.chart) {
      this.chart.destroy(); // Destroy the chart when the component is destroyed to prevent memory leaks
    }
  }

  private createDoughnutChart(): void {
    const ctx = this.chartCanvas.nativeElement.getContext('2d');
    if (ctx) {
      const config: ChartConfiguration<'doughnut'> = {
        type: 'doughnut',
        data: this.chartData, // Uses the input data
        options: this.constantChartOptions // Uses the internally defined constant options
      };
      this.chart = new Chart(ctx, config);
    }
  }

  private updateDoughnutChart(): void {
    if (this.chart) {
      this.chart.data = this.chartData; // Update chart's data property
      this.chart.update(); // Tells Chart.js to redraw the chart with the new data
    }
  }
}