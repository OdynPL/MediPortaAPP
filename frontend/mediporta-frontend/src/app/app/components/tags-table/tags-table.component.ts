import { Component, OnInit } from '@angular/core';
import { TagDto } from '../../DTO/TagDto';
import { TagsService } from '../../services/tags-service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-tags-table',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './tags-table.component.html',
  styleUrl: './tags-table.component.css'
})
export class TagsTableComponent implements OnInit {
  tags: TagDto[] = [];
  pageNumber = 1;
  pageSize = 20;
  sortBy = 'name';
  ascending = true;
  loading = false;

  constructor(private tagsService: TagsService) {}

  ngOnInit(): void {
    this.loadTags();
  }

  loadTags(): void {
    this.loading = true;
    this.tagsService.getTags(this.pageNumber, this.pageSize, this.sortBy, this.ascending)
      .subscribe({
        next: data => { this.tags = data; this.loading = false; },
        error: err => { console.error(err); this.tags = []; this.loading = false; }
      });
  }

  toggleSort(field: string): void {
    if (this.sortBy === field) {
      this.ascending = !this.ascending;
    } else {
      this.sortBy = field;
      this.ascending = true;
    }
    this.pageNumber = 1;
    this.loadTags();
  }

  refresh(): void {
    this.tagsService.refreshTags().subscribe({
      next: () => this.loadTags(),
      error: err => console.error('Refresh error', err)
    });
  }

  prevPage(): void {
    if (this.pageNumber > 1) {
      this.pageNumber--;
      this.loadTags();
    }
  }

  nextPage(): void {
    this.pageNumber++;
    this.loadTags();
  }
}
