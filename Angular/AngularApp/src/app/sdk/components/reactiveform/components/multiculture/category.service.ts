import { Injectable } from "@angular/core";
import { EntityType, CategoryEntity, BlogEntity } from './category.model';

// Entity factory service
@Injectable()
export class EntityFactoryService {
  createEntity(type: 'category' | 'blog'): EntityType {
    switch (type) {
      case 'category':
        return this.createCategoryEntity();
      case 'blog':
        return this.createBlogEntity();
      default:
        throw new Error(`Unknown entity type: ${type}`);
    }
  }

  private createCategoryEntity(): CategoryEntity {
    return {
      id: 0,
      culture: 'en',
      title: '',
      sub_title: '',
      description: '',
    };
  }

  private createBlogEntity(): BlogEntity {
    return {
      id: 0,
      culture: 'en',
      title: '',
      short_description: '',
      description: '',
    };
  }
}