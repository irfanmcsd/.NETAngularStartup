export interface BaseEntity {
  id: number;
  culture: string;
}

export interface CategoryEntity extends BaseEntity {
  title: string;
  sub_title: string;
  description: string;
}

export interface BlogEntity extends BaseEntity {
  title: string;
  short_description: string;
  description: string;
}

export type EntityType = CategoryEntity | BlogEntity;
