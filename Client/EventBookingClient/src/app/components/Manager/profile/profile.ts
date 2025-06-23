import { HttpClient } from '@angular/common/http';
import { Component, signal, computed, ViewChild, ElementRef, AfterViewInit } from '@angular/core';
import { AnalyticsService } from '../../../services/Analytics/analytics.service';
import { CommonModule } from '@angular/common';
import Chart from 'chart.js/auto';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './profile.html',
  styleUrls: ['./profile.css']
})
export class Profile implements AfterViewInit {
  @ViewChild('earningsChart') chartRef!: ElementRef<HTMLCanvasElement>;
  private chartInstance: any;
  
  earningsTableSignal = signal<{ event: string; amount: number }[]>([]);
  earningsTable = computed(() => this.earningsTableSignal());
  earningsTableTotal = computed(() => this.earningsTableSignal().reduce((sum, item) => sum + item.amount, 0));

  constructor(
    private http: HttpClient,
    private analyticsService: AnalyticsService
  ) {}

  ngOnInit(): void {
    this.loadEarnings();
  }

  ngAfterViewInit() {
    this.renderChart();
  }

  loadEarnings() {
    this.analyticsService.getMyEarning().subscribe({
      next: (res: any) => {
        const data = Object.fromEntries(Object.entries(res.data).slice(1));
        const table = Object.entries(data).map(([event, amount]) => ({
          event,
          amount: Number(amount)
        }));
        this.earningsTableSignal.set(table);
        this.renderChart();
      },
      error: () => alert('Failed to load earnings.')
    });
  }

  renderChart() {
    if (!this.chartRef || !this.earningsTable().length) return;
    
    // Destroy previous chart if exists
    if (this.chartInstance) {
      this.chartInstance.destroy();
    }

    const ctx = this.chartRef.nativeElement.getContext('2d');
    if (!ctx) return;

    const labels = this.earningsTable().map(e => e.event);
    const data = this.earningsTable().map(e => e.amount);

    this.chartInstance = new Chart(ctx, {
      type: 'pie',
      data: {
        labels: labels,
        datasets: [{
          data: data,
          backgroundColor: [
            '#FF6384', '#36A2EB', '#FFCE56', '#4BC0C0', '#9966FF',
            '#FF9F40', '#8AC926', '#1982C4', '#6A4C93', '#F15BB5'
          ]}
        ]
      }
    });
  }
}